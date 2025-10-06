using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Xunit;

namespace SeleniumTests;

public sealed class TaskCreationWithProjectAssignment : BaseTest
{
    [Fact]
    public void Should_create_task_with_project_assignment_and_display_it_in_project()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/missions/5";
        const string taskName = "task to project";
        const string projectName = "Autotest project";
        const string projectPageUrl = "https://protasks.test.app/#/projects/page/1551?projectTab=5&fromFolder=3&missionView=1";
        

        var locators = new
        {
            CreateTaskButton = By.XPath("//span[text()='Создать задачу']"),
            ModalDialog = By.XPath("//*[contains(@style, 'z-index') and number(translate(substring-after(substring-before(@style, ';'), 'z-index:'), ' ', '')) > 2399]"),
            TaskInput = By.XPath(".//input[starts-with(@id, 'input-')]"),
            CreateButton = By.XPath("/html/body/div[2]/div[6]/div[2]/div/div[4]/button[2]/span[3]"),
            ProjectLocationButton = By.XPath("//*[text()=' Место в проекте ']"),
            ProjectCombobox = By.XPath("//div[@role='combobox'][not(.//label[contains(., 'Ответственный')])]"),
            ProjectSearchInput = By.XPath(".//input[@type='text']"),
            ProjectOption = By.XPath($"//div[contains(@class, 'v-list-item-title') and normalize-space()='{projectName}']"),
            TaskInGrid = By.XPath($"//*[contains(text(), '{taskName}')]"),
            ContextMenuOption = By.XPath(".//span[text()='Удалить']"),
            ConfirmDeleteButton = By.XPath(".//span[text()=' Удалить']")
            
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        CreateTaskWithProject(locators, taskName, projectName);
        VerifyTaskInProject(projectPageUrl, taskName);
        DeleteTask(taskName);
        VerifyTaskDeleted(taskName);
    }
    
    private void CreateTaskWithProject(dynamic locators, string taskName, string projectName)
    {
        ClickElement(locators.CreateTaskButton);
        
        var modal = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ModalDialog);
            return element.Displayed ? element : null;
        });
        
        // Заполняем название задачи
        var inputField = modal.FindElement(locators.TaskInput);
        inputField.Click();
        inputField.Clear();
        inputField.SendKeys(taskName);
        
        // Выбираем проект
        ClickElement(locators.ProjectLocationButton);
        
        var projectCombobox = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ProjectCombobox);
            return element.Displayed && element.Enabled ? element : null;
        });
        
        projectCombobox.Click();
        
        var searchInput = projectCombobox.FindElement(locators.ProjectSearchInput);
        searchInput.Clear();
        searchInput.SendKeys(projectName);
        
        var projectOption = Wait.Until(drv => 
        {
            var element = drv.FindElement(locators.ProjectOption);
            return element.Displayed ? element : null;
        });
        
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", projectOption);
        
        // Создаем задачу
        ClickElement(locators.CreateButton);    
    }

    private void VerifyTaskInProject(string projectPageUrl, string taskName)
    {
        // Переходим на страницу проекта
        Driver.Navigate().GoToUrl(projectPageUrl);
        WaitForPageLoad();
        
        // Проверяем наличие задачи в проекте
        var taskElement = Wait.Until(drv => 
        {
            var element = drv.FindElement(By.XPath($"//*[contains(text(), '{taskName}')]"));
            return element.Displayed ? element : null;
        });
        
        Assert.NotNull(taskElement);
        Assert.Contains(taskName, taskElement.Text);
    }

    private void DeleteTask(string taskName)
    {
        ((IJavaScriptExecutor)Driver).ExecuteScript("window.location.href = 'https://protasks.test.app/#/projects/page/1551?projectTab=5&fromFolder=3&missionView=1'");
        WaitForPageLoad();
        
        // Находим задачу для контекстного меню
        var taskElement = Wait.Until(drv => 
        {
            var element = drv.FindElement(By.XPath($"//span[text()='{taskName}']"));
            return element.Displayed ? element : null;
        });
        
        // Правый клик для открытия контекстного меню
        new Actions(Driver)
            .MoveToElement(taskElement)
            .ContextClick()
            .Perform();
        
        // Выбираем "Удалить" из контекстного меню
        var deleteOption = Wait.Until(drv => 
        {
            var element = drv.FindElement(By.XPath(".//span[text()='Удалить']"));
            return element.Displayed && element.Enabled ? element : null;
        });
        
        deleteOption.Click();
        
        // Подтверждаем удаление
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