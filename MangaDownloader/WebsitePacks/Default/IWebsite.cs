using MangaDownloader.app.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaDownloader.WebsitePacks.Default
{
    internal interface IWebsite
    {
        string Name { get; }
        string[] Url { get; }
        void GetMangas(ref Dictionary<string, string> mangas, ref System.ComponentModel.BackgroundWorker process);
        void GetEpisodes(KeyValuePair<string, string> Manga, ref Dictionary<string, string> episodes, ref System.ComponentModel.BackgroundWorker process);
        void Download(string DownloadPath, KeyValuePair<string, string> Episode);
    }
}
