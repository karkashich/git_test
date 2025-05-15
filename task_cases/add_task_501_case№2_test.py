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

def test_501_task_name():
    # Указываем полный путь к chromedriver.exe
    driver_path = os.path.abspath("webdriver/chromedriver.exe")  # или r"webdriver\chromedriver.exe"
    service = Service(executable_path=driver_path)
    driver = webdriver.Chrome(service=service)

    def add_task_noproject_case1(driver):
        driver.get("https://portal.test.app/#/projects/missions/5")
        # обязательный блок так как вход не запоминается \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        wait = WebDriverWait(driver, 10)
        log_in = wait.until(EC.presence_of_element_located((By.ID, "social-keycloak-oidc")))
        log_in.click()
        # \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        WebDriverWait(driver, 3)

        span_text = "Создать задачу"
        span_element = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, f"//span[text()='{span_text}']"))
        )
        span_element.click()

        modal = WebDriverWait(driver, 10).until(
            EC.visibility_of_element_located((By.XPATH, "//*[contains(@style, 'z-index') and number(substring-before(substring-after(@style, 'z-index:'), ';')) > 2399]")))
        
        input_field = modal.find_element(By.ID, "input-0")
        
        input_field.click()
        input_field.send_keys("taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2taskcase№2t")

        # \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        span_btn = " Создать "
        span_task = WebDriverWait(modal, 10).until(
            EC.element_to_be_clickable((By.XPATH, f".//span[text()='{span_btn}']"))
        )
        span_task.click()

        WebDriverWait(driver, 10)

        validation_error= WebDriverWait(driver, 10).until(
            EC.presence_of_element_located((By.XPATH, "//*[contains(text(), 'Максимальная длина наименования - 500 символов')]"))
        )
        assert validation_error

    add_task_noproject_case1(driver)
    driver.quit()
