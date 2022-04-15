using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MangaDownloader.WebsitePacks.v1_0;

namespace MangaDownloader.WebsitePacks.Default
{
    internal class WebsitesList
    {
        internal List<List<IWebsite>> Websites = new List<List<IWebsite>>();

        internal WebsitesList()
        {
            Websites.Add(new List<IWebsite> {new Manmantai()});
        }
    }
}
