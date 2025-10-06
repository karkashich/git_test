using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumTests;

public sealed class ProjectAddNewParticipant : BaseTest
{
    [Fact]
    public void ShouldAddAndRemoveProjectParticipant()
    {
        // Константы и локаторы
        var projectUrl = $"{AppUrlOptions.BaseUrl}/#/projects/page/1551?projectTab=5&missionView=1";
        const string userName = "Богданов Павел Алексеевич";
        const string userRole = "Руководитель проекта";

        var locators = new
        {
            AddParticipantButton = By.XPath("//span[text()='Добавить участника']"),
            RoleField = By.XPath("//label[contains(., 'Роль в команде')]/following::div[contains(@class, 'v-field__input')]"),
            RoleOption = By.XPath($"//div[@class='v-list-item-title' and text()='{userRole}']"),
            NameField = By.Id("input-v-0-11"),
            UserOption = By.XPath($"//*[contains(text(), '{userName}')]"),
            SaveButton = By.XPath("//span[@class='v-btn__content' and text()=' Сохранить ']"),
            UserElement = By.XPath($"//*[contains(text(), '{userName}')]"),
            DeleteButton = By.XPath("//*[contains(text(), 'Удалить участника')]"),
            ConfirmDeleteButton = By.XPath("//span[text()=' Удалить']")
        };

        // Выполнение теста
        NavigateToUrl(projectUrl);
        LoginToAccount();
        NavigateToUrl(projectUrl);
        AddParticipant(locators, userName, userRole);
        VerifyParticipantAdded(locators.UserElement, userName);
        RemoveParticipant(locators, userName);
        VerifyParticipantRemoved(locators.UserElement);
    }
    
    private void AddParticipant(dynamic locators, string userName, string userRole)
    {
        ClickElement(locators.AddParticipantButton);
        WaitForPageLoad();
        
        ClickElement(locators.RoleField);
        ClickElement(locators.RoleOption);
        
        EnterText(locators.NameField, userName);
        ClickElement(locators.UserOption);
        
        ClickElement(locators.SaveButton);
        WaitForPageLoad();
    }

    private void VerifyParticipantAdded(By userLocator, string expectedUserName)
    {
        var userElement = Wait.Until(drv => drv.FindElement(userLocator));
        Assert.NotNull(userElement);
        Assert.Contains(expectedUserName, userElement.Text);
    }

    private void RemoveParticipant(dynamic locators, string userName)
    {
        var userElement = Wait.Until(drv => drv.FindElement(locators.UserElement));
        userElement.Click();
        
        ClickElement(locators.DeleteButton);
        ClickElement(locators.ConfirmDeleteButton);
        WaitForPageLoad();
    }

    private void VerifyParticipantRemoved(By userLocator)
    {
        Wait.Until(drv =>
        {
            var elements = drv.FindElements(userLocator);
            return elements.Count == 0;
        });
    }
}