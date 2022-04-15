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
        internal WebDriver driver;

        internal Driver(bool hidden)
        {
            ChromeOptions options = new ChromeOptions();
            if (hidden) options.AddArgument("headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            this.driver = new ChromeDriver(service, options);
        }

        internal void Exit()
        {
            driver.Close();
            driver.Quit();
        }
    }
}
