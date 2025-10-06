using OpenQA.Selenium;
using Xunit;

namespace SeleniumTests;

public sealed class ProjectCreateMilestone : BaseTest
{
    [Fact]
    public void ShouldAddAndRemoveProjectMilestone()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/page/1551?projectTab=4&fromFolder=3";
        const string milestoneName = "autotest milestone";
        const string milestoneType = "Очередь поставки";
        const string milestoneDate = "01.10.2025";

        var locators = new
        {
            AddMilestoneButton = By.XPath("//span[text()='Добавить веху']"),
            TypeField = By.XPath("//label[contains(., 'Тип вехи')]/following::div[contains(@class, 'v-field__input')]"),
            TypeOption = By.XPath($"//div[@class='v-list-item-title' and text()='{milestoneType}']"),
            NameField = By.XPath("//label[contains(., 'Наименование')]/following::div[contains(@class, 'v-field__input')]"),
            NameInput = By.Id("input-v-0-3"),
            DateField = By.XPath("//label[contains(., 'Дата наступления')]/following::div[contains(@class, 'v-field__input')]"),
            DateInput = By.Id("input-v-0-5"),
            SaveButton = By.XPath("//span[text()=' Сохранить ']"),
            MilestoneElement = By.XPath($"//*[contains(text(), '{milestoneName}')]"),
            DeleteButton = By.XPath("//*[contains(text(), 'Удалить веху')]"),
            ConfirmDeleteButton = By.XPath("//span[text()=' Удалить']")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        AddMilestone(locators, milestoneName, milestoneType, milestoneDate);
        VerifyMilestoneAdded(locators.MilestoneElement, milestoneName);
        RemoveMilestone(locators, milestoneName);
        VerifyMilestoneRemoved(locators.MilestoneElement);
    }
    

    private void AddMilestone(dynamic locators, string milestoneName, string milestoneType, string milestoneDate)
    {
        ClickElement(locators.AddMilestoneButton);
        WaitForPageLoad();
        
        ClickElement(locators.TypeField);
        ClickElement(locators.TypeOption); 
        
        ClickElement(locators.DateField);
        EnterText(locators.DateInput, milestoneDate);
        
        ClickElement(locators.SaveButton);
        
        EnterText(locators.NameInput, milestoneName);
        
        ClickElement(locators.SaveButton);
        
        WaitForPageLoad();
    }

    private void VerifyMilestoneAdded(By milestoneLocator, string expectedMilestoneName)
    {
        var milestoneElement = Wait.Until(drv => drv.FindElement(milestoneLocator));
        Assert.NotNull(milestoneElement);
        Assert.Contains(expectedMilestoneName, milestoneElement.Text);
    }

    private void RemoveMilestone(dynamic locators, string milestoneName)
    {
        var milestoneElement = Wait.Until(drv => drv.FindElement(locators.MilestoneElement));
        milestoneElement.Click();
        
        ClickElement(locators.DeleteButton);
        ClickElement(locators.ConfirmDeleteButton);
        WaitForPageLoad();
    }

    private void VerifyMilestoneRemoved(By milestoneLocator)
    {
        Wait.Until(drv =>
        {
            var elements = drv.FindElements(milestoneLocator);
            return elements.Count == 0;
        });
    }

}