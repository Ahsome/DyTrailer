using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DyTrailer {
    internal class AppleDownloader : IDownloader {
        public async Task Download<T> (string url, T media) where T : IMedia {

            if (url != "") {
                WebClient webClient = new WebClient ();
                webClient.Headers.Add ("User-Agent", "Quick_time/7.6.2");
                webClient.Headers.Add ("Accept", "text/html");
                webClient.Proxy = null;
                //TODO: Code to actually use the URL, not just the hardcoded one, This also requires renaming url to contain "h"
                await webClient.DownloadFileTaskAsync ("http://movietrailers.apple.com/movies/marvel/avengers-infinity-war/avengers-infinity-war-trailer-2_h1080p.mov", $"{Path.Combine(media.FileDirectory, media.FileName)}.mov");
            }
        }
    }
}