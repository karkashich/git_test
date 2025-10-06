using OpenQA.Selenium;
using Xunit;
using OpenQA.Selenium.Interactions;

namespace SeleniumTests;

public sealed class TaskCreationBasedOnExisting : BaseTest
{
    [Fact]
    public void Should_create_task_based_on_existing_task()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string baseTaskName = "base on";

        var locators = new
        {
            BaseTask = By.XPath($"//*[contains(text(), '{baseTaskName}')]"),
            ContextMenu = By.ClassName("ag-menu"),
            BasedOnOption = By.XPath("//div[@class='ag-menu-option']//span[text()='Создать задачу на основе']"),
            ModalDialog = By.CssSelector("div.v-card.dialog-card"),
            SaveButton = By.XPath("//button//span[contains(@class, 'v-btn__content') and text()=' Сохранить ']"),
            CreatedTasks = By.XPath($"//*[contains(text(), '{baseTaskName}')]")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        CreateTaskBasedOnExisting(locators, baseTaskName);
        VerifyTasksCreated(locators.CreatedTasks, baseTaskName, expectedCount: 2);
    }

    private void  CreateTaskBasedOnExisting(dynamic locators, string baseTaskName)
    {
        // Находим базовую задачу
        Thread.Sleep(1000);
        
        var baseTask = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.BaseTask);
            return element.Displayed ? element : null;
        });
        
        // Правый клик для открытия контекстного меню
        new Actions(Driver)
            .MoveToElement(baseTask)
            .ContextClick(baseTask)
            .Perform();
        
        // Ждем появления контекстного меню
        var contextMenu = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ContextMenu);
            return element.Displayed ? element : null;
        });
        
        // Выбираем "Создать задачу на основе"
        var basedOnOption = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.BasedOnOption);
            return element.Displayed && element.Enabled ? element : null;
        });
        
        basedOnOption.Click();
        
        // Ждем появления модального окна
        var modal = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ModalDialog);
            return element.Displayed ? element : null;
        });
        
        // Нажимаем кнопку "Сохранить"
        var saveButton = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.SaveButton);
            return element.Displayed && element.Enabled ? element : null;
        });
        
        saveButton.Click();
        Thread.Sleep(500);
        WaitForPageLoad();
        
    }

    private void VerifyTasksCreated(By tasksLocator, string expectedTaskName, int expectedCount)
    {
        var tasks = Wait.Until(drv => 
        {
            var elements = drv.FindElements(tasksLocator);
            return elements.Count >= expectedCount ? elements : null;
        });
        
        Assert.NotNull(tasks);
        Assert.Equal(expectedCount, tasks.Count);
        
        foreach (var task in tasks)
        {
            Assert.Contains(expectedTaskName, task.Text);
        }
    }
}