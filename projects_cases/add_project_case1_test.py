from selenium import webdriver
from selenium.webdriver.common.by import By
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import os
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.chrome.service import Service  
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.action_chains import ActionChains


def test_opentaskpage():
    # Указываем полный путь к chromedriver.exe
    driver_path = os.path.abspath("webdriver/chromedriver.exe")  # или r"webdriver\chromedriver.exe"
    service = Service(executable_path=driver_path)
    driver = webdriver.Chrome(service=service)

    def add_project_case1(driver):
        driver.get("https://portal.test.app/#/projects/missions/5")
        # обязательный блок так как вход не запоминается \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        wait = WebDriverWait(driver, 10)
        log_in = wait.until(EC.presence_of_element_located((By.ID, "social-keycloak-oidc")))
        log_in.click()
        # \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        WebDriverWait(driver, 3)

        span_text = "Создать проект"
        span_element = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, f"//span[text()='{span_text}']"))
        )
        span_element.click()

        modal = WebDriverWait(driver, 10).until(
            EC.visibility_of_element_located((By.XPATH, "//*[contains(@style, 'z-index') and number(substring-before(substring-after(@style, 'z-index:'), ';')) > 2399]")))
        
        WebDriverWait(driver, 3)

        # \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        # почему то не кликает по элементу скорее всего не находит элемент и не нажимает на него
        # надо найти элемент который кликать надо 

        input_field = WebDriverWait(modal, 10).until(
            EC.visibility_of_element_located((By.CSS_SELECTOR, "textarea.v-field__input:not([readonly])"))
        )

        input_field.send_keys("new project autotest case №1")

        span_btn = "Сохранить"
        span_task = WebDriverWait(modal, 10).until(
            EC.element_to_be_clickable((By.XPATH, f".//span[text()='{span_btn}']"))
        )
        WebDriverWait(driver, 10)
        span_task.click()

        WebDriverWait(driver, 10)

        new_task= WebDriverWait(driver, 10).until(
            EC.presence_of_element_located((By.XPATH, "//*[contains(text(), 'new project autotest case №1')]"))
        )
        assert new_task
        
    add_project_case1(driver)
    driver.quit()