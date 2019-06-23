using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DyTrailer {
    public class YoutubeRentScraper : IScraper {
        string url = "";
        YoutubeDownloader youtubeDownloader;
        public YoutubeRentScraper () {
            youtubeDownloader = new YoutubeDownloader();
            
        }
        public void SetTrailerUrl <T> (T trailer) where T : ITrailer {
            string youtubeUrl = GetYoutubeRentLinkAsync (trailer);
            this.url = youtubeUrl;
        }

        private string GetYoutubeRentLinkAsync<T> (T trailer) where T:ITrailer {
            string originalLink = GetOriginalYoutubeLink (trailer);
            string convertedLink = ConvertToRentLink (originalLink);
            
            //TODO: Remember to remove WriteLine
            Console.WriteLine (convertedLink);
            return convertedLink;
        }

        private string ConvertToRentLink (string originalLink) {
            var httpClient = new HttpClient ();
            var htmlDocument = new HtmlDocument ();

            htmlDocument.LoadHtml (httpClient.GetStringAsync (originalLink).Result);

            HtmlNode htmlNode = null;
            foreach (var node in htmlDocument.DocumentNode.SelectNodes ("//script")) {
                var text = node.GetDirectInnerText ();
                if (node.GetDirectInnerText ().Contains ("var ytplayer = ytplayer")) {
                    htmlNode = node;
                    break;
                }
            }

            int linkIdPosition = htmlNode.GetDirectInnerText ().IndexOf ("trailerVideoId") + 19;
            string linkId = $"http://youtu.be/{htmlNode.GetDirectInnerText().Substring(linkIdPosition, 11)}";

            if (( httpClient.GetAsync (linkId).Result).IsSuccessStatusCode) {
                return linkId;
            } else {
                //TODO: Specifiy what this exception is for
                throw new Exception ();
            }
        }

        public void StartDownload<T>(T trailer) where T:ITrailer
        {
            YoutubeDownloader youtubeDownloader = new YoutubeDownloader();
            youtubeDownloader.Download(url, trailer);
        }

        private string GetOriginalYoutubeLink<T> (T trailer) where T:ITrailer {
            string originalLinkId = "";
            string query = Uri.EscapeUriString ($"{trailer.Name} {trailer.Type}");
            string queryUrl = $"https://www.youtube.com/results?search_query={query}";

            var httpClient = new HttpClient ();
            var htmlDocument = new HtmlDocument ();

            htmlDocument.LoadHtml (httpClient.GetStringAsync(queryUrl).Result);

            try {
                originalLinkId = htmlDocument.DocumentNode.SelectNodes ("//div[contains(@class, 'yt-lockup yt-lockup-tile yt-lockup-movie-vertical-poster vve-check clearfix yt-uix-tile')]") [0].Attributes["data-context-item-id"].Value;
            } catch (Exception) {

            }

            return $"https://www.youtube.com/watch?v={originalLinkId}";
        }
    }
}