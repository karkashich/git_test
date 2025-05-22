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

def test_deletetask():
    # Указываем полный путь к chromedriver.exe
    driver_path = os.path.abspath("webdriver/chromedriver.exe")  # или r"webdriver\chromedriver.exe"
    service = Service(executable_path=driver_path)
    driver = webdriver.Chrome(service=service)
    driver.maximize_window()

    def add_task_noproject_case1(driver):
        print('nachalo')
        driver.get("https://portal.test.app/#/projects/missions/5")
        # обязательный блок так как вход не запоминается \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        wait = WebDriverWait(driver, 10)
        log_in = wait.until(EC.presence_of_element_located((By.ID, "social-keycloak-oidc")))
        log_in.click()
        # \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        driver.get("https://portal.test.app/#/projects/missions/5")

        WebDriverWait(driver, 10)

        driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")

        task_from_case1 = WebDriverWait(driver, 10).until(
            EC.presence_of_element_located((By.XPATH, "//a[text()='new autotask case №1']"))
        )
        task_from_case1 = driver.find_element(By.XPATH, "//a[text()='new autotask case №1']")
        task_from_case1.click()

        WebDriverWait(driver, 10)

        for window_handle in driver.window_handles:
            if window_handle != driver:
                driver.switch_to.window(window_handle)
                break

        delete_btn = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, f".//span[text()='Удалить']"))
        )
        delete_btn.click()

        delete_btn = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, f".//span[text()=' Удалить']"))
        )
        delete_btn.click()

        assert WebDriverWait(driver, 10).until(
            EC.invisibility_of_element_located((By.XPATH, "//*[contains(text(), 'new autotask case №1')]"))
        )

    add_task_noproject_case1(driver)
    driver.quit()
