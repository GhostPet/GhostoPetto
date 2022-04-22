using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        string[] Url = { "https://www.manmantai.com/", "https://m.manmantai.com/" };

        string IWebsite.Name => Name;
        string[] IWebsite.Url => Url;

        public void GetMangas(ref Dictionary<string, string> mangas, ref System.ComponentModel.BackgroundWorker process)
        {
            var driver = new Driver(true, true);
            var browser = driver.driver;

            while (true)
            {
                try
                {
                    browser.Navigate().GoToUrl(Url[0] + "list/");
                    break;
                }
                catch (TimeoutException) { }
            }

            string Last = browser.FindElement(By.CssSelector("#w1 > div > ul > li.last > a")).GetAttribute("href");
            int Counter = 1;
            while (browser.Url != Last)
            {
                while (true)
                {
                    try
                    {
                        browser.Navigate().GoToUrl(Url[0] + "list_" + ++Counter + "/");
                        break;
                    }
                    catch (TimeoutException) { }
                }

                foreach (var element in browser.FindElements(By.ClassName("item-lg")))
                {
                    string MangaName = element.FindElement(By.CssSelector("p > a")).Text;
                    string MangaUrl = element.FindElement(By.CssSelector("p > a")).GetAttribute("href");
                    mangas.Add(MangaName, MangaUrl);
                    process.ReportProgress(0);
                    if (process.CancellationPending)
                    {
                        driver.Exit();
                        return;
                    }
                }
            }
            driver.Exit();
            return;
        }

        public void GetEpisodes(KeyValuePair<string, string> Manga, ref Dictionary<string, string> episodes, ref System.ComponentModel.BackgroundWorker process)
        {
            var driver = new Driver(true, true);
            var browser = driver.driver;
            browser.Navigate().GoToUrl(Manga.Value);
            foreach (var element in browser.FindElements(By.CssSelector("#chapter-list-4 > li")))
            {
                string EpisodeName = element.FindElement(By.CssSelector("a > span")).Text;
                string EpisodeUrl = element.FindElement(By.CssSelector("a")).GetAttribute("href");
                if (episodes != null)
                {
                    if (episodes.ContainsKey(EpisodeName))
                    {
                        int Counter = 1;
                        EpisodeName += " - ";
                        while (episodes.ContainsKey(EpisodeName + Counter.ToString()))
                        {
                            Counter++;
                        }
                        EpisodeName += Counter.ToString();
                    }
                }
                episodes.Add(EpisodeName, EpisodeUrl);
                process.ReportProgress(0);
                if (process.CancellationPending)
                {
                    driver.Exit();
                    return;
                }
            }
            driver.Exit();
            return;
        }

        public void Download(string DownloadPath, KeyValuePair<string, string> Episode)
        {
            var driver = new Driver(true, false);
            var browser = driver.driver;

            while (true)
            {
                try
                {
                    // If Mobile Website:
                    if (browser.Url.StartsWith("https://m.manmantai.com/"))
                    {
                        string newUrl = Episode.Value.Replace("https://m.manmantai.com/", "https://www.manmantai.com/");
                        browser.Navigate().GoToUrl(newUrl);
                    }
                    // If Desktop Website:
                    else browser.Navigate().GoToUrl(Episode.Value);
                    break;
                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            string MangaName = browser.FindElement(By.CssSelector("body > div.chapter-view > div.w996.title.pr > h1")).Text;
            WebClient downloader = new WebClient();
            int Counter = 1;
            while (true)
            {
                string ImageUrl = browser.FindElement(By.Id("images")).FindElement(By.TagName("img")).GetAttribute("src");
                string ImageName = String.Format("{0:000}", Counter++);

                
                if (!Directory.Exists(DownloadPath + "\\ManManTai")) Directory.CreateDirectory(DownloadPath + "\\ManManTai");
                if (!Directory.Exists(DownloadPath + "\\ManManTai\\" + MangaName)) Directory.CreateDirectory(DownloadPath + "\\ManManTai\\" + MangaName);
                if (!Directory.Exists(DownloadPath + "\\ManManTai\\" + MangaName + "\\" + Episode.Key)) Directory.CreateDirectory(DownloadPath + "\\ManManTai\\" + MangaName + "\\" + Episode.Key);

                downloader.DownloadFile(new Uri(ImageUrl), DownloadPath + "\\ManManTai\\" + MangaName + "\\" + Episode.Key + "\\" + ImageName + ".jpg");

                //// Close Ad
                //try
                //{
                //    browser.FindElement(By.CssSelector("#hbidbox > img:nth-child(3)")).Click();
                //    System.Threading.Thread.Sleep(10);
                //}
                //catch (Exception)
                //{
                //    System.Threading.Thread.Sleep(10);
                //}

                browser.FindElement(By.ClassName("nextPage")).Click();
                System.Threading.Thread.Sleep(200);

                try
                {
                    var alert = browser.SwitchTo().Alert();
                    alert.Accept();
                    break;
                }
                catch (Exception)
                {
                    if (!browser.Url.Contains("#")) break;
                }
            }

            driver.Exit();
            return;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
