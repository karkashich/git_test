using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests;

public sealed class TaskNameLengthValidation : BaseTest
{
    [Fact]
    public void Should_show_validation_error_when_task_name_exceeds_500_characters()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string expectedError = "Максимальная длина наименования - 500 символов";
        const string longTaskName = "taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№23";

        var locators = new
        {
            CreateTaskButton = By.XPath("//span[text()='Создать задачу']"),
            ModalDialog = By.XPath("//*[contains(@style, 'z-index') and number(translate(substring-after(substring-before(@style, ';'), 'z-index:'), ' ', '')) > 2399]"),
            TaskInput = By.XPath(".//input[starts-with(@id, 'input-')]"),
            CreateButton = By.XPath("/html/body/div[2]/div[6]/div[2]/div/div[4]/button[2]/span[3]"),
            ValidationError = By.XPath($"//*[contains(text(), '{expectedError}')]")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        CreateTaskWithLongName(locators, longTaskName);
        VerifyValidationError(locators.ValidationError, expectedError);
    }
    
    private void CreateTaskWithLongName(dynamic locators, string taskName)
    {
        ClickElement(locators.CreateTaskButton);
        
        var modal = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ModalDialog);
            return element.Displayed ? element : null;
        });
        
        var inputField = modal.FindElement(locators.TaskInput);
        inputField.Click();
        inputField.Clear();
        inputField.SendKeys(taskName);
        
        ClickElement(locators.CreateButton);
        WaitForPageLoad();
    }

    private void VerifyValidationError(By errorLocator, string expectedError)
    {
        var errorElement = Wait.Until(modal => 
        {
            var element = modal.FindElement(errorLocator);
            return element.Displayed ? element : null;
        });
        
        Assert.NotNull(errorElement);
        Assert.Contains(expectedError, errorElement.Text);
    }
}