using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests;

public sealed class ProjectCreatonValidation: BaseTest
{
    [Fact]
    public void ShouldShowValidationErrorWhenProjectCreateWithEmptyName()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string expectedError = "Введите наименование проекта";

        var locators = new
        {
            CreateProjectButton = By.XPath("//span[text()='Создать проект']"),
            ModalDialog = By.XPath("//*[contains(@style, 'z-index') and number(translate(substring-after(substring-before(@style, ';'), 'z-index:'), ' ', '')) > 2399]"),
            CreateButton = By.XPath("//span[text()='Сохранить']"),
            ValidationError = By.XPath($"//*[contains(text(), '{expectedError}')]")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        CreateEmptyProject(locators);
        VerifyValidationError(locators.ValidationError, expectedError);
    }
    
    private void CreateEmptyProject(dynamic locators)
    {
        ClickElement(locators.CreateProjectButton);
        
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