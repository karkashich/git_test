using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests;

public sealed class TaskCreationValidation : BaseTest
{
    [Fact]
    public void ShouldShowValidationErrorWhenTaskCreateWithEmptyName()
    {
        // Константы и локаторы
        var ProjectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string expectedError = "Введите наименование задачи";

        var locators = new
        {
            CreateTaskButton = By.XPath("//span[text()='Создать задачу']"),
            ModalDialog = By.XPath("//*[contains(@style, 'z-index') and number(translate(substring-after(substring-before(@style, ';'), 'z-index:'), ' ', '')) > 2399]"),
            TaskInput = By.XPath(".//input[starts-with(@id, 'input-')]"),
            CreateButton = By.XPath("/html/body/div[2]/div[6]/div[2]/div/div[4]/button[2]/span[3]"),
            ValidationError = By.XPath($"//*[contains(text(), '{expectedError}')]")
        };
        
        NavigateToUrl(ProjectUrl);
        LoginToAccount();
        NavigateToUrl(ProjectUrl);
        CreateEmptyTask(locators);
        VerifyValidationError(locators.ValidationError, expectedError);
    }

    private void CreateEmptyTask(dynamic locators)
    {
        ClickElement(locators.CreateTaskButton);
        
        var modal = Wait.Until(drv => drv.FindElement(locators.ModalDialog));
        var inputField = modal.FindElement(locators.TaskInput);
        
        inputField.Click();
        inputField.Clear();
        inputField.SendKeys("");
        
        ClickElement(locators.CreateButton);
        WaitForPageLoad();
    }

    private void VerifyValidationError(By errorLocator, string expectedError)
    {
        var errorElement = Wait.Until(drv => drv.FindElement(errorLocator));
        Assert.NotNull(errorElement);
        Assert.Contains(expectedError, errorElement.Text);
    }
}