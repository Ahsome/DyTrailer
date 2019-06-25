using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DyTrailer {
    public class YoutubeRentScraper : IScraper {

        YoutubeDownloader youtubeDownloader = new YoutubeDownloader ();

        public List < (string Url, string Type) > ListOfVideos {
            get;
            private set;
        } = new List < (string, string) > ();

        public List<string> SupportedMedia { get; } = new List<string> () { "trailer" };
        public List<string> SupportedContent { get; } = new List<string> () { "movie" };

        public void SetPossibleVideos<T> (T content) where T : IContent {
            ListOfVideos.Clear ();
            string youtubeUrl = GetYoutubeRentLink (content);
            if (youtubeUrl != null) {
                ListOfVideos.Add ((youtubeUrl, "trailer"));
            }
        }

        private string GetYoutubeRentLink<T> (T content) where T : IContent {
            string originalLink = GetOriginalYoutubeLink (content);
            if (originalLink != null) {
                string convertedLink = ConvertToRentLink (originalLink);
                if (convertedLink != null) {
                    return convertedLink;
                }
            }
            return null;
        }

        private string ConvertToRentLink (string originalLink) {
            HtmlDocument queryHtml = UtilClass.GetPageHtml (originalLink);
            HtmlNode node = queryHtml.DocumentNode.SelectNodes ("//script").Where (x => x.GetDirectInnerText ().Contains ("var ytplayer = ytplayer")).First ();

            int linkIdPosition = node.GetDirectInnerText ().IndexOf ("trailerVideoId") + 19;
            string linkId = $"http://youtu.be/{node.GetDirectInnerText().Substring(linkIdPosition, 11)}";
            if (!linkId.Contains (" ")) {
                return linkId;
            } else {
                return null;
            }

        }

        private string GetOriginalYoutubeLink<T> (T content) where T : IContent {
            string originalLinkId = null;
            string query = Uri.EscapeUriString ($"{content.Name} {content.Type}");
            string queryUrl = $"https://www.youtube.com/results?search_query={query}";

            var queryHtml = UtilClass.GetPageHtml (queryUrl);
            //TODO: Check to see if the first one is the correct one
            try {
                originalLinkId = queryHtml?.DocumentNode?.SelectNodes ("//div[contains(@class, 'yt-lockup yt-lockup-tile yt-lockup-movie-vertical-poster vve-check clearfix yt-uix-tile')]") [0]?.Attributes["data-context-item-id"]?.Value;
            } catch (Exception) {
                Console.WriteLine ($"WARNING: {content.Name} does not exist on YouTube Rent. Continuing on");
            }
            if (originalLinkId != null) {
                return $"https://www.youtube.com/watch?v={originalLinkId}";
            } else {
                return null;
            }

        }

        public IDownloader GetDownloader () {
            return youtubeDownloader;
        }
    }
}