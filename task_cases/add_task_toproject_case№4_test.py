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

def test_add_task_to_project():
    # Указываем полный путь к chromedriver.exe
    driver_path = os.path.abspath("webdriver/chromedriver.exe")  # или r"webdriver\chromedriver.exe"
    service = Service(executable_path=driver_path)
    driver = webdriver.Chrome(service=service)
    driver.maximize_window()

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
        input_field.send_keys("task to project")

        place_in_project = WebDriverWait(modal, 10).until(
            EC.element_to_be_clickable((By.XPATH, "//*[text()=' Место в проекте ']"))
        )
        place_in_project.click()

        input_project = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.CSS_SELECTOR, "input#input-25"))
        )
        input_project.click()

        input_project.send_keys("Autotest project")

        # 2. Ищем элемент по ТОЧНОМУ тексту (регистрозависимо)
        project = WebDriverWait(driver, 10).until(
            EC.presence_of_element_located(
                (By.XPATH, "//div[contains(@class, 'v-list-item-title') and normalize-space()='Autotest project']")
            )
        )
        
        # 3. Кликаем через JavaScript (самый надежный способ)
        driver.execute_script("arguments[0].click();", project)

        span_btn = " Создать "
        span_task = WebDriverWait(modal, 10).until(
            EC.element_to_be_clickable((By.XPATH, f".//span[text()='{span_btn}']"))
        )

        span_task.click()

        # \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        WebDriverWait(driver, 10)

        driver.execute_script("window.location.href = 'https://portal.test.app/#/projects/page/1104?projectTab=5&missionView=1'")


        new_task= WebDriverWait(driver, 10).until(
            EC.presence_of_element_located((By.XPATH, "//*[contains(text(), 'task to project')]"))
        )
        assert new_task

    add_task_noproject_case1(driver)
    driver.quit()
