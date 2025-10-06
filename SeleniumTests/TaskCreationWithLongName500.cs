using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Xunit;

namespace SeleniumTests;

public sealed class TaskCreationWithLongName : BaseTest
{
    [Fact]
    public void Should_create_task_with_long_name_and_display_it_in_grid()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string longTaskName = "taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2";

        var locators = new
        {
            CreateTaskButton = By.XPath("//span[text()='Создать задачу']"),
            ModalDialog = By.XPath("//*[contains(@style, 'z-index') and number(translate(substring-after(substring-before(@style, ';'), 'z-index:'), ' ', '')) > 2399]"),
            TaskInput = By.XPath(".//input[starts-with(@id, 'input-')]"),
            CreateButton = By.XPath("/html/body/div[2]/div[6]/div[2]/div/div[4]/button[2]/span[3]"),
            TaskInGrid = By.XPath("//*[contains(text(), 'taskcase№2')]"),
            ContextMenuOption = By.XPath("//div[@class='ag-menu-option']//span[text()='Удалить']"),
            ConfirmDeleteButton = By.XPath(".//span[text()=' Удалить']")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        CreateTask(locators, longTaskName);
        VerifyTaskCreated(locators.TaskInGrid, longTaskName);
        DeleteTaskViaContextMenu(longTaskName);
        VerifyTaskDeleted(longTaskName);
    }
    
    private void CreateTask(dynamic locators, string taskName)
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

    private void VerifyTaskCreated(By taskLocator, string expectedTaskName)
    {
        var taskElement = Wait.Until(drv => 
        {
            var element = drv.FindElement(taskLocator);
            return element.Displayed ? element : null;
        });
        
        Assert.NotNull(taskElement);
        Assert.Contains(expectedTaskName, taskElement.Text);
        
        Driver.Navigate().Refresh();
        WaitForPageLoad();
    }

    private void DeleteTaskViaContextMenu(string taskName)
    {
        var taskElement = Wait.Until(drv => 
        {
            var element = drv.FindElement(By.XPath($"//*[contains(text(), '{taskName}')]"));
            return element.Displayed ? element : null;
        });
        
        new Actions(Driver)
            .MoveToElement(taskElement)
            .ContextClick()
            .Perform();
        
        var contextMenu = Wait.Until(drv => 
        {
            var element = drv.FindElement(By.ClassName("ag-menu"));
            return element.Displayed ? element : null;
        });
        
        var deleteOption = Wait.Until(drv => 
        {
            var element = drv.FindElement(By.XPath("//div[@class='ag-menu-option']//span[text()='Удалить']"));
            return element.Displayed && element.Enabled ? element : null;
        });
        
        deleteOption.Click();
        
        var confirmDeleteButton = Wait.Until(drv => 
        {
            var element = drv.FindElement(By.XPath(".//span[text()=' Удалить']"));
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