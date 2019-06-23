using System.IO;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DyTrailer {
    internal class AppleScraper : IScraper {
        string appleUrl = "";
        AppleDownloader appleDownloader = new AppleDownloader();
        public AppleScraper () { }

        public void SetTrailerUrl<T> (T trailer) where T : ITrailer {
            string cleanedName = UtilClass.CleanTrailerName (trailer.Name).Replace (' ', '+');
            string appleFindUrl = $"https://trailers.apple.com/trailers/home/scripts/quickfind.php?q={cleanedName}";
            dynamic appleFindJson = UtilClass.GetDynamicJson(appleFindUrl);

            appleUrl = $"https://trailers.apple.com{appleFindJson.results[0].location}";
        }

        public void StartDownload<T> (T trailer) where T : ITrailer {
            string listUrl = "";
            dynamic filmData = UtilClass.GetDynamicJson($"{appleUrl}/data/page.json");
            foreach (dynamic clip in filmData.clips) {
                dynamic videoType = clip.title.ToString ();
                if (videoType.Contains ("Trailer")) {
                    listUrl  = clip.versions.enus.sizes.hd1080.src;
                    break;
                }
            }

            appleDownloader.Download(listUrl, trailer);
        }
    }
}