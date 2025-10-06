
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumTests;

public sealed class ProjectReportManagement : BaseTest
{    
    [Fact]
    public void Should_add_and_remove_report_from_project()
    {
        // Константы и локаторы
        var projectReportsUrl = $"{AppUrlOptions.BaseUrl}/#/projects/page/1551?projectTab=10&missionView=1";
        const string reportName = "Autotest report";

        var locators = new
        {
            AddReportButton = By.XPath(".//span[text()='Добавить отчет']"),
            TemplateOption = By.XPath("//div[contains(@class, 'v-list-item-title') and contains(text(), 'Выбрать отчет из готовых шаблонов')]"),
            ReportSearchInput = By.Id("input-v-0-0"),
            ReportOption = By.XPath($"//*[contains(text(), '{reportName}')]"),
            AddToProjectButton = By.XPath(".//span[text()=' Добавить в проект ']"),
            DeleteReportButton = By.XPath("//button[.//i[contains(@class, 'material-icons') and text()='close']]"),
            ConfirmDeleteButton = By.XPath(".//span[text()=' Исключить']"),
            ReportInList = By.XPath($"//*[contains(text(), '{reportName}')]")
        };

        // Выполнение теста
        NavigateToUrl(projectReportsUrl);
        LoginToAccount();
        NavigateToUrl(projectReportsUrl);
        AddReportToProject(locators, reportName);
        VerifyReportAdded(locators.ReportInList, reportName);
        RemoveReportFromProject(locators);
        VerifyReportRemoved(reportName);
    }
    
    private void AddReportToProject(dynamic locators, string reportName)
    {
        ClickElement(locators.AddReportButton);
        
        ClickElement(locators.TemplateOption);
        
        var inputField = new WebDriverWait(Driver, TimeSpan.FromSeconds(10))
            .Until(drv => drv.FindElement(By.Id("input-v-0-0")));
        inputField.Click();
        inputField.Click();
        
        EnterText(locators.ReportSearchInput, reportName);
        
        var reportOption = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ReportOption);
            return element.Displayed ? element : null;
        });
        
        reportOption.Click();
        
        ClickElement(locators.AddToProjectButton);
        WaitForPageLoad();
    }

    private void VerifyReportAdded(By reportLocator, string expectedReportName)
    {
        var reportElement = Wait.Until(drv => 
        {
            var element = drv.FindElement(reportLocator);
            return element.Displayed ? element : null;
        });
        
        Assert.NotNull(reportElement);
        Assert.Contains(expectedReportName, reportElement.Text);
    }

    private void RemoveReportFromProject(dynamic locators)
    {
        var deleteButton = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.DeleteReportButton);
            return element.Displayed && element.Enabled ? element : null;
        });
        
        deleteButton.Click();
        Thread.Sleep(500);
        
        var confirmDeleteButton = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ConfirmDeleteButton);
            return element.Displayed && element.Enabled ? element : null;
        });
        
        confirmDeleteButton.Click();
        Thread.Sleep(1000);
        WaitForPageLoad();
    }

    private void VerifyReportRemoved(string reportName)
    {
        Wait.Until(drv =>
        {
            try
            {
                var elements = drv.FindElements(By.XPath($"//*[contains(text(), '{reportName}')]"));
                return elements.Count == 0;
            }
            catch
            {
                return true;
            }
        });
    }
}