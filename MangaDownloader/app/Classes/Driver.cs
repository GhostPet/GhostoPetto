using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaDownloader.app.Classes
{
    internal class Driver
    {
        internal static List<Driver> DriverList = new List<Driver>();

        internal WebDriver driver;
        internal Driver(bool IsHidden, bool DisableImg)
        {
            ChromeOptions options = new ChromeOptions();
            if (IsHidden) options.AddArgument("headless");
            options.AddArgument("--window-size=1920,16394");
            //if (DisableImg) options.AddUserProfilePreference("managed_default_content_settings.images", 2);
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            this.driver = new ChromeDriver(service, options);
            this.driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(300);
            DriverList.Add(this);
        }

        internal void Exit()
        {
            driver.Close();
            driver.Quit();
            DriverList.Remove(this);
        }

        internal static void ExitAll()
        {
            foreach (Driver driver in DriverList)
            {
                driver.driver.Close();
                driver.driver.Quit();
            }
            DriverList.Clear();
        }
    }
}
