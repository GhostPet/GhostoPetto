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
        List<List<string>> GetMangas(Driver driver);
        List<List<string>> GetEpisodes(Driver driver, string DetailslUrl);
        bool Download(Driver driver, string DownloadPath, string EpisodeName, string ReaderUrl);
    }
}
