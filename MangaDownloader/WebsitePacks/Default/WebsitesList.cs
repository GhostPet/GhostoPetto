using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MangaDownloader.WebsitePacks.v1_0;

namespace MangaDownloader.WebsitePacks.Default
{
    internal static class WebsitesList
    {
        internal static List<IWebsite> Websites = new List<IWebsite>
        {
            // You must add all your websites inside here.
            new Manmantai()
        };

    }
}
