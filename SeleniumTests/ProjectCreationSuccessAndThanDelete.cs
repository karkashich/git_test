using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests;

public sealed class ProjectCreationSuccessAndThenDelete : BaseTest
{
    [Fact]
    public void Should_create_project_and_then_delete_it()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string projectName = "new project autotest case №1";

        var locators = new
        {
            CreateProjectButton = By.XPath("//span[text()='Создать проект']"),
            ModalDialog = By.XPath("//*[contains(@style, 'z-index') and number(translate(substring-after(substring-before(@style, ';'), 'z-index:'), ' ', '')) > 2399]"),
            ProjectNameTextarea = By.CssSelector("textarea.v-field__input:not([readonly])"),
            SaveButton = By.XPath("//span[text()='Сохранить']"),
            ProjectInList = By.XPath($"//*[contains(text(), '{projectName}')]"),
            DeleteButton = By.XPath("//button[contains(@class, 'v-btn') and .//span[contains(., 'Удалить')]]"),
            ConfirmDeleteButton = By.XPath(".//span[text()=' Удалить']")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        CreateProject(locators, projectName);
        VerifyProjectCreated(locators.ProjectInList, projectName);
        DeleteProject(locators);
        VerifyProjectDeleted(projectName);
    }
    
    private void CreateProject(dynamic locators, string projectName)
    {
        ClickElement(locators.CreateProjectButton);
        
        var modal = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ModalDialog);
            return element.Displayed ? element : null;
        });
        
        var textarea = modal.FindElement(locators.ProjectNameTextarea);
        textarea.Click();
        textarea.Clear();
        textarea.SendKeys(projectName);
        
        ClickElement(locators.SaveButton);
        WaitForPageLoad();
    }

    private void VerifyProjectCreated(By projectLocator, string expectedProjectName)
    {
        var projectElement = Wait.Until(drv => 
        {
            var element = drv.FindElement(projectLocator);
            return element.Displayed ? element : null;
        });
        
        Assert.NotNull(projectElement);
        Assert.Contains(expectedProjectName, projectElement.Text);
    }

    private void DeleteProject(dynamic locators)
    {
        ClickElement(locators.DeleteButton);
        
        var confirmDeleteButton = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ConfirmDeleteButton);
            return element.Displayed && element.Enabled ? element : null;
        });
        
        confirmDeleteButton.Click();
        WaitForPageLoad();
        
        Thread.Sleep(500);
    }

    private void VerifyProjectDeleted(string projectName)
    {
        // Переходим на страницу проектов
        ((IJavaScriptExecutor)Driver).ExecuteScript("window.location.href = 'https://protasks.test.app/#/projects/3'");
        Thread.Sleep(500);
        WaitForPageLoad();
        
        // Проверяем, что проект удален
        Wait.Until(drv =>
        {
            try
            {
                var elements = drv.FindElements(By.XPath($"//*[contains(text(), '{projectName}')]"));
                return elements.Count == 0;
            }
            catch
            {
                return true;
            }
        });
    }
}