using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

public abstract class BaseTest : IDisposable
{
    protected readonly ChromeDriver Driver;
    protected readonly WebDriverWait Wait;
    protected readonly CredentialsOptions CredentialsOptions;
    protected readonly AppUrlOptions AppUrlOptions;
    protected readonly IConfiguration Configuration;
    

    protected BaseTest() 
    {
        // Конфигурация путей
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var driverPath = Path.Combine(baseDirectory, "WebDrivers");
        var configPath = Path.Combine(baseDirectory, "appsettings.json");

        ValidatePaths(driverPath, configPath);

        // Настройка ChromeDriver
        var options = new ChromeOptions();
        
        options.AddArgument("--start-maximized");
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);
        
        Driver = new ChromeDriver(driverPath, options);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));

        // Загрузка конфигурации
        Configuration = BuildConfiguration(configPath);
        CredentialsOptions = Configuration.GetSection("credentials").Get<CredentialsOptions>() ??
                            throw new InvalidOperationException("Учетные данные не найдены");
        AppUrlOptions = Configuration.GetSection("AppUrl").Get<AppUrlOptions>() ??
                 throw new InvalidCastException("Url приложения не найден");
    }
    
    private static void ValidatePaths(string driverPath, string configPath)
    {
        if (!File.Exists(Path.Combine(driverPath, "chromedriver.exe")))
            throw new FileNotFoundException($"ChromeDriver не найден по пути: {driverPath}");

        if (!File.Exists(configPath))
            throw new FileNotFoundException($"Файл конфигурации не найден по пути: {configPath}");
    }

    private static IConfiguration BuildConfiguration(string configPath)
    {
        return new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(configPath)!)
            .AddJsonFile(Path.GetFileName(configPath))
            .Build();
    }

    protected void NavigateToUrl(string url)
    {
        Driver.Navigate().GoToUrl(url);
        WaitForPageLoad();
    }

    protected void WaitForPageLoad()
    {
        Wait.Until(driver =>
        {
            var readyState = ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState");
            return readyState?.ToString() == "complete";
        });
    }

    protected void LoginToAccount()
    {
        var locators = new
        {
            Username = By.Id("username"),
            Password = By.Id("password"),
            LoginButton = By.Id("kc-login")
        };

        EnterText(locators.Username, CredentialsOptions.Username ?? throw new InvalidOperationException("Не найден логин"));
        EnterText(locators.Password, CredentialsOptions.Password ?? throw new InvalidOperationException("Не найден пароль"));
        ClickElement(locators.LoginButton);
        WaitForPageLoad();
    }

    protected void EnterText(By locator, string text)
    {
        var element = Wait.Until(drv => drv.FindElement(locator));
        element.Clear();
        element.SendKeys(text);
    }

    protected void ClickElement(By locator)
    {
        var element = Wait.Until(drv =>
        {
            var el = drv.FindElement(locator);
            return el.Displayed && el.Enabled ? el : null;
        });
        element?.Click();
    }

    public virtual void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}