using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DyTrailer {
    internal class AppleScraper : IScraper {
        AppleDownloader appleDownloader = new AppleDownloader ();

        public List < (string Url, string Type) > ListOfVideos {
            get;
            private set;
        }

        public List<string> SupportedMedia {get; } = new List<string>(){"trailer"};
        public List<string> SupportedContent {get; } = new List<string>(){"Movie"};

        public AppleScraper () {
            ListOfVideos = new List < (string, string) > ();
        }

        public void SetPossibleVideos<T> (T content) where T : IContent {
            string cleanedName = UtilClass.CleanMediaName (content.Name).Replace (' ', '+');
            string appleFindUrl = $"https://trailers.apple.com/trailers/home/scripts/quickfind.php?q={cleanedName}";
            dynamic appleFindJson = UtilClass.GetDynamicJson (appleFindUrl);
            //TODO: Do check to see if the first one is actually the one we want
            string appleDataUrl = $"https://trailers.apple.com{appleFindJson.results[0].location}";

            dynamic filmData = UtilClass.GetDynamicJson ($"{appleDataUrl}/data/page.json");
            foreach (dynamic clip in filmData.clips) {
                dynamic videoType = clip.title.ToString ();
                ListOfVideos.Add ((clip.versions.enus.sizes.hd1080.src, videoType.ToLower ()));
            }
        }

        public IDownloader GetDownloader () {
            return appleDownloader;
        }
    }
}