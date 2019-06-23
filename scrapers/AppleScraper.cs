using System.IO;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DyTrailer {
    internal class AppleScraper : IScraper {
        string appleUrl = "";
        public AppleScraper () { }

        public void SetTrailerUrl<T> (T trailer) where T : ITrailer {
            string cleanedName = UtilClass.CleanTrailerName (trailer.Name).Replace (' ', '+');
            string appleFindUrl = $"https://trailers.apple.com/trailers/home/scripts/quickfind.php?q={cleanedName}";

            var client = new RestClient (appleFindUrl);
            var request = new RestRequest (Method.GET);
            request.AddParameter ("undefined", "{}", ParameterType.RequestBody);

            string response = client.Execute (request).Content;
            dynamic responseJson = JObject.Parse (response);
            Console.WriteLine (responseJson.results[0].location);

            appleUrl = $"https://trailers.apple.com{responseJson.results[0].location}";
            //return responseJson.results[0];

        }

        public void StartDownload<T> (T trailer) where T : ITrailer {
            //List<String> listUrl = new List<String> ();
            string listUrl = "";
            var client = new RestClient ($"{appleUrl}/data/page.json");
            var request = new RestRequest (Method.GET);
            request.AddParameter ("undefined", "{}", ParameterType.RequestBody);

            string response = client.Execute (request).Content;
            dynamic filmData = JObject.Parse (response);
            foreach (dynamic clip in filmData.clips) {
                dynamic videoType = clip.title.ToString ();
                if (videoType.Contains ("Trailer")) {
                    string fullHdSource = clip.versions.enus.sizes.hd1080.src;
                    listUrl = fullHdSource;
                }
            }

            if (listUrl != "") {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent","Quick_time/7.6.2");
                webClient.Headers.Add("Accept","text/html");
                webClient.Proxy = null;
                webClient.DownloadFile("http://movietrailers.apple.com/movies/marvel/avengers-infinity-war/avengers-infinity-war-trailer-2_h1080p.mov", $"{Path.Combine(trailer.FileDirectory, trailer.FileName)}.mov");
                Console.WriteLine(webClient.Headers);
            }
        }
    }
}
/* 
{
    "error": false,
    "results": [{
        "title": "Avengers: Infinity War",
        "releasedate": "Fri, 27 Apr 2018 00:00:00 -0700",
        "studio": "Marvel Studios",
        "poster": "\/trailers\/marvel\/avengers-infinity-war\/images\/poster.jpg",
        "moviesite": "",
        "location": "\/trailers\/marvel\/avengers-infinity-war\/",
        "urltype": "html",
        "director": "Array",
        "rating": "PG-13",
        "genre": ["Action and Adventure", "Science Fiction"],
        "actors": ["\u00a0"],
        "trailers": [{
                "type": "Marvel Legacy",
                "postdate": "Thu, 19 Apr 2018 00:00:00 -0700",
                "exclusive": false,
                "hd": true
            },
            {
                "type": "Featurette- Family",
                "postdate": "Thu, 12 Apr 2018 00:00:00 -0700",
                "exclusive": false,
                "hd": true
            },
            {
                "type": "Featurette 2",
                "postdate": "Mon, 09 Apr 2018 00:00:00 -0700",
                "exclusive": false,
                "hd": true
            },
            {
                "type": "Trailer 2",
                "postdate": "Fri, 16 Mar 2018 00:00:00 -0700",
                "exclusive": false,
                "hd": true
            },
            {
                "type": "Big Game Spot",
                "postdate": "Mon, 05 Feb 2018 00:00:00 -0800",
                "exclusive": false,
                "hd": true
            },
            {
                "type": "Trailer",
                "postdate": "Wed, 29 Nov 2017 00:00:00 -0800",
                "exclusive": false,
                "hd": true
            },
            {
                "type": "Featurette",
                "postdate": "Mon, 13 Feb 2017 00:00:00 -0800",
                "exclusive": false,
                "hd": true
            }
        ]
    }]
} */