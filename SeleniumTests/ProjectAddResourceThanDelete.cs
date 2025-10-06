using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumTests;

public sealed class ProjectAddResourceThanDelete : BaseTest
{
    [Fact]
    public void ShouldAddAndRemoveProjectResource()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/page/1551?projectTab=8&missionView=1";
        const string resourceName = "Autotest resource";

        var locators = new
        {
            AddResourceButton = By.XPath("//span[text()='Добавить ресурс']"),
            ResourceNameInput = By.Id("input-v-0-0"),
            SaveButton = By.XPath("//span[@class='v-btn__content' and text()=' Сохранить ']"),
            ResourceElement = By.XPath($"//*[contains(text(), '{resourceName}')]"),
            DeleteButton = By.XPath("//*[contains(text(), 'Удалить ресурс')]"),
            ConfirmDeleteButton = By.XPath("//span[text()=' Удалить']")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        AddResource(locators, resourceName);
        VerifyResourceAdded(locators.ResourceElement, resourceName);
        RemoveResource(locators, resourceName);
        VerifyResourceRemoved(locators.ResourceElement);
    }

    private void AddResource(dynamic locators, string resourceName)
    {
        ClickElement(locators.AddResourceButton);
        WaitForPageLoad();
        
        var fieldInput = new WebDriverWait(Driver, TimeSpan.FromSeconds(10))
            .Until(drv => drv.FindElement(locators.ResourceNameInput));
        fieldInput.Click(); 
        
        EnterText(locators.ResourceNameInput, resourceName);
        
        ClickElement(locators.SaveButton);
        WaitForPageLoad();
    }

    private void VerifyResourceAdded(By resourceLocator, string expectedResourceName)
    {
        var resourceElement = Wait.Until(drv => drv.FindElement(resourceLocator));
        Assert.NotNull(resourceElement);
        Assert.Contains(expectedResourceName, resourceElement.Text);
    }

    private void RemoveResource(dynamic locators, string resourceName)
    {
        var resourceElement = Wait.Until(drv => drv.FindElement(locators.ResourceElement));
        resourceElement.Click();
        
        ClickElement(locators.DeleteButton);
        ClickElement(locators.ConfirmDeleteButton);
        WaitForPageLoad();
    }

    private void VerifyResourceRemoved(By resourceLocator)
    {
        Wait.Until(drv =>
        {
            var elements = drv.FindElements(resourceLocator);
            return elements.Count == 0;
        });
    }
}