using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumTests;

public sealed class TaskCreationinProject : BaseTest
{
    [Fact]
    public void TaskCreationInProjectAndThenDelete()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/page/1551?projectTab=5&missionView=1";
        const string taskName = "project_case№4";

        var locators = new
        {
            CreateTaskButton = By.XPath(".//span[text()='Добавить задачу']"),
            TaskInput = By.Id("input-v-0-0"),
            CreateButton = By.XPath(".//span[text()=' Создать ']"),
            TaskInGrid = By.XPath($"//*[contains(text(), '{taskName}')]"),
            DeleteButton = By.XPath("//button[contains(@class, 'v-btn') and .//span[contains(., 'Удалить')]]"),
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
        
        var inputField = new WebDriverWait(Driver, TimeSpan.FromSeconds(10))
            .Until(drv => drv.FindElement(locators.TaskInput));
        
        inputField.Click();
        inputField.Clear();
        inputField.SendKeys(taskName);
        
        ClickElement(locators.CreateButton);
        WaitForPageLoad();
        Thread.Sleep(5000);
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

