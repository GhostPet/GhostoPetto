using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaDownloader.app.Classes;
using MangaDownloader.WebsitePacks.Default;
using OpenQA.Selenium;

namespace MangaDownloader.WebsitePacks.v1_0
{
    internal class Manmantai : IWebsite
    {
        string Name = "Manmantai";
        string[] Url = { "https://www.manmantai.com/" };

        string IWebsite.Name => Name;
        string[] IWebsite.Url => Url;

        public List<List<string>> GetMangas(Driver driver)
        {
            var browser = driver.driver;
            browser.Navigate().GoToUrl(Url + "list/");
            string Last = browser.FindElement(By.CssSelector("#w1 > div > ul > li.last > a")).GetAttribute("href");

            List < List<string> > mangas = new List<List<string>>();
            int Counter = 0;
            while (browser.Url != Last)
            {
                browser.Navigate().GoToUrl(Url + "list_" + ++Counter + "/");
                foreach (var element in browser.FindElements(By.ClassName("item-lg")))
                {
                    string MangaName = element.FindElement(By.CssSelector("p > a")).Text;
                    string MangaUrl = element.FindElement(By.CssSelector("p > a")).GetAttribute("href");
                    mangas.Add(new List<string> { MangaName, MangaUrl });
                }
            }
            return mangas;
        }

        public List<List<string>> GetEpisodes(Driver driver, string DetailslUrl)
        {
            var browser = driver.driver;
            List<List<string>> episodes = new List<List<string>>();
            foreach (var element in browser.FindElements(By.CssSelector("#chapter-list-4 > li")))
            {
                string EpisodeName = element.FindElement(By.CssSelector("a > span")).Text;
                string EpisodeUrl = element.FindElement(By.CssSelector("a")).GetAttribute("href");
                episodes.Add(new List<string> { EpisodeName, EpisodeUrl });
            }
            return episodes;
        }

        public bool Download(Driver driver, string DownloadPath, string EpisodeName, string ReaderUrl)
        {
            throw new NotImplementedException();
        }

    }
}
