using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using Xunit;

namespace SeleniumTests;

public sealed class SecondTaskCreationinProject : BaseTest
{
    [Fact]
    public void TaskCreationInProjectAndThenDeleteByContextMenu()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/page/1551?projectTab=5&missionView=1";
        const string taskName = "project_case№5";

        var locators = new
        {
            AddTaskButton = By.XPath("//*[@id=\"gantt\"]/div/div/div[1]/div[1]/div/div[2]/div/div/button"),
            TaskNameInput = By.Id("input-v-0-0"),
            CreateButton = By.XPath("//span[text()=' Создать ']"),
            TaskElement = By.XPath($"//*[contains(text(), '{taskName}')]"),
            DeleteOption = By.XPath("//span[text()='Удалить']"),
            ConfirmDeleteButton = By.XPath("//span[text()=' Удалить']")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        
        AddTask(locators, taskName);
        VerifyTaskAdded(locators.TaskElement, taskName);
        RemoveTask(locators, taskName);
        VerifyTaskRemoved(locators.TaskElement);
    }
    
    private void RightClickElement(By locator)
    {
        var element = Wait.Until(drv =>
        {
            var el = drv.FindElement(locator);
            return el.Displayed && el.Enabled ? el : null;
        });
        
        // Создаем Actions для выполнения правого клика
        var actions = new OpenQA.Selenium.Interactions.Actions(Driver);
        actions.ContextClick(element).Perform();
    }

    private void AddTask(dynamic locators, string taskName)
    {
        ClickElement(locators.AddTaskButton);
        WaitForPageLoad();
        
        EnterText(locators.TaskNameInput, taskName);
        
        ClickElement(locators.CreateButton);
        WaitForPageLoad();
    }

    private void VerifyTaskAdded(By taskLocator, string expectedTaskName)
    {
        var taskElement = Wait.Until(drv => drv.FindElement(taskLocator));
        Assert.NotNull(taskElement);
        Assert.Contains(expectedTaskName, taskElement.Text);
    }

    private void RemoveTask(dynamic locators, string taskName)
    {
        // Обновляем страницу для уверенности, что задача отобразилась
        Driver.Navigate().Refresh();
        WaitForPageLoad();

        var taskElement = Wait.Until(drv => drv.FindElement(locators.TaskElement));
        
        // Правый клик по задаче для вызова контекстного меню
        RightClickElement(locators.TaskElement);
        
        // Выбор опции "Удалить" из контекстного меню
        ClickElement(locators.DeleteOption);
        
        // Подтверждение удаления
        ClickElement(locators.ConfirmDeleteButton);
        WaitForPageLoad();
    }

    private void VerifyTaskRemoved(By taskLocator)
    {
        Wait.Until(drv =>
        {
            var elements = drv.FindElements(taskLocator);
            return elements.Count == 0;
        });
    }
}