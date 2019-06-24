using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DyTrailer {
    public class YoutubeRentScraper : IScraper {

        YoutubeDownloader youtubeDownloader = new YoutubeDownloader ();

        public List < (string Url, string Type) > ListOfVideos {
            get;
            private set;
        }

        public List<string> SupportedMedia {get; } = new List<string>(){"trailer"};
        public List<string> SupportedContent {get; } = new List<string>(){"movie"};

        public YoutubeRentScraper () {
            ListOfVideos = new List < (string, string) > ();
        }
        public void SetPossibleVideos<T> (T content) where T : IContent {
            ListOfVideos.Clear();
            string youtubeUrl = GetYoutubeRentLink (content);
            ListOfVideos.Add((youtubeUrl, "trailer"));
        }

        private string GetYoutubeRentLink<T> (T content) where T : IContent {
            string originalLink = GetOriginalYoutubeLink (content);
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

        private string GetOriginalYoutubeLink<T> (T content) where T : IContent {
            string originalLinkId = "";
            string query = Uri.EscapeUriString ($"{content.Name} {content.Type}");
            string queryUrl = $"https://www.youtube.com/results?search_query={query}";

            var queryHtml = UtilClass.GetPageHtml (queryUrl);
            //TODO: Check to see if the first one is the correct one
            originalLinkId = queryHtml.DocumentNode.SelectNodes ("//div[contains(@class, 'yt-lockup yt-lockup-tile yt-lockup-movie-vertical-poster vve-check clearfix yt-uix-tile')]") [0].Attributes["data-context-item-id"].Value;

            return $"https://www.youtube.com/watch?v={originalLinkId}";
        }

        public IDownloader GetDownloader()
        {
            return youtubeDownloader;
        }
    }
}