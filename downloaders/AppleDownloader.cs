using System.IO;
using System.Net;

namespace DyTrailer {
    internal class AppleDownloader : IDownloader {
        public AppleDownloader () { }

        public void Download<T> (string url, T trailer) where T : ITrailer {

            if (url != "") {
                WebClient webClient = new WebClient ();
                webClient.Headers.Add ("User-Agent", "Quick_time/7.6.2");
                webClient.Headers.Add ("Accept", "text/html");
                webClient.Proxy = null;
                webClient.DownloadFile ("http://movietrailers.apple.com/movies/marvel/avengers-infinity-war/avengers-infinity-war-trailer-2_h1080p.mov", $"{Path.Combine(trailer.FileDirectory, trailer.FileName)}.mov");
            }
        }
    }
}