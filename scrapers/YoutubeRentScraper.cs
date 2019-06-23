using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DyTrailer {
    public class YoutubeRentScraper : IScraper {
        string youtubeRentUrl = "";
        YoutubeDownloader youtubeDownloader = new YoutubeDownloader ();
        public YoutubeRentScraper () { }
        public void SetTrailerUrl<T> (T trailer) where T : ITrailer {
            string youtubeUrl = GetYoutubeRentLink (trailer);
            youtubeRentUrl = youtubeUrl;
        }

        private string GetYoutubeRentLink<T> (T trailer) where T : ITrailer {
            string originalLink = GetOriginalYoutubeLink (trailer);
            string convertedLink = ConvertToRentLink (originalLink);
            return convertedLink;
        }

        private string ConvertToRentLink (string originalLink) {
            HtmlDocument queryHtml = UtilClass.GetPageHtml (originalLink);
            HtmlNode htmlNode = null;
            
            foreach (var node in queryHtml.DocumentNode.SelectNodes ("//script")) {
                if (node.GetDirectInnerText ().Contains ("var ytplayer = ytplayer")) {
                    htmlNode = node;
                    break;
                }
            }

            int linkIdPosition = htmlNode.GetDirectInnerText ().IndexOf ("trailerVideoId") + 19;
            string linkId = $"http://youtu.be/{htmlNode.GetDirectInnerText().Substring(linkIdPosition, 11)}";
            return linkId;
        }

        public void StartDownload<T> (T trailer) where T : ITrailer {
            youtubeDownloader.Download (youtubeRentUrl, trailer);
        }

        private string GetOriginalYoutubeLink<T> (T trailer) where T : ITrailer {
            string originalLinkId = "";
            string query = Uri.EscapeUriString ($"{trailer.Name} {trailer.Type}");
            string queryUrl = $"https://www.youtube.com/results?search_query={query}";

            var queryHtml = UtilClass.GetPageHtml (queryUrl);
            originalLinkId = queryHtml.DocumentNode.SelectNodes ("//div[contains(@class, 'yt-lockup yt-lockup-tile yt-lockup-movie-vertical-poster vve-check clearfix yt-uix-tile')]") [0].Attributes["data-context-item-id"].Value;

            return $"https://www.youtube.com/watch?v={originalLinkId}";
        }
    }
}