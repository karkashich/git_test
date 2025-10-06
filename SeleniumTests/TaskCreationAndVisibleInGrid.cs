using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests;

public sealed class TaskCreationAndVisibleInGrid : BaseTest
{
    [Fact]
    public void Should_create_task_and_display_it_in_grid()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string taskName = "new autotask case №1";

        var locators = new
        {
            CreateTaskButton = By.XPath("//span[text()='Создать задачу']"),
            ModalDialog = By.XPath("//*[contains(@style, 'z-index') and number(translate(substring-after(substring-before(@style, ';'), 'z-index:'), ' ', '')) > 2399]"),
            TaskInput = By.XPath(".//input[starts-with(@id, 'input-')]"),
            CreateButton = By.XPath("/html/body/div[2]/div[6]/div[2]/div/div[4]/button[2]/span[3]"),
            TaskInGrid = By.XPath($"//*[contains(text(), '{taskName}')]"),
            DeleteButton = By.XPath(".//span[text()='Удалить']"),
            ConfirmDeleteButton = By.XPath(".//span[text()=' Удалить']")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        CreateTask(locators, taskName);
        VerifyTaskCreated(locators.TaskInGrid, taskName);
        DeleteTask(locators, taskName);
        VerifyTaskDeleted(taskName);
    }
    
    private void CreateTask(dynamic locators, string taskName)
    {
        ClickElement(locators.CreateTaskButton);
        
        var modal = Wait.Until(drv => drv.FindElement(locators.ModalDialog));
        var inputField = modal.FindElement(locators.TaskInput);
        
        inputField.Click();
        inputField.Clear();
        inputField.SendKeys(taskName);
        
        ClickElement(locators.CreateButton);
        WaitForPageLoad();
    }

    private void VerifyTaskCreated(By taskLocator, string expectedTaskName)
    {
        var taskElement = Wait.Until(drv => 
        {
            var element = drv.FindElement(taskLocator);
            return element.Displayed ? element : null;
        });
        
        Assert.NotNull(taskElement);
        Assert.Contains(expectedTaskName, taskElement.Text);
    }

    private void DeleteTask(dynamic locators, string taskName)
    {
        var deleteButton = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.DeleteButton);
            return element.Displayed && element.Enabled ? element : null;
        });
        deleteButton.Click();
        
        var confirmDeleteButton = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ConfirmDeleteButton);
            return element.Displayed && element.Enabled ? element : null;
        });
        confirmDeleteButton.Click();
        
        WaitForPageLoad();
    }

    private void VerifyTaskDeleted(string taskName)
    {
        Wait.Until(drv =>
        {
            try
            {
                var elements = drv.FindElements(By.XPath($"//*[contains(text(), '{taskName}')]"));
                return elements.Count == 0;
            }
            catch
            {
                return true;
            }
        });
    }
}