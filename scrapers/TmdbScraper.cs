using System;
using System.Collections.Generic;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace DyTrailer {
    public class TmdbScraper : IScraper {
        TMDbClient tmdbClient;
        YoutubeDownloader youtubeDownloader = new YoutubeDownloader ();
        //private List<dynamic> ListOfVideos { get; set; }
        public List < (string Url, string Type) > ListOfVideos {
            get;
            private set;
        }

        public TmdbScraper () {
            //TODO: Insert ApiKey
            tmdbClient = new TMDbClient ("11c74be7cd64fd121c64222bc9e8c27d");
            ListOfVideos = new List < (string, string) > ();
        }

        public void SetPossibleVideos<T> (T content) where T : IContent {
            ListOfVideos.Clear ();
            FindTmdbVideos (content);
        }

        private void FindTmdbVideos<T> (T content) where T : IContent {
            var searchResults = tmdbClient.SearchMovieAsync (content.Name).Result;
            foreach (SearchMovie searchResult in searchResults.Results) {
                if (MatchYear (searchResult, content) && MatchTitle (searchResult, content)) {
                    StoreTmdbVideos (searchResult);
                }
            }
        }

        private void StoreTmdbVideos (SearchMovie searchResult) {
            dynamic tmdbJson = GetTmdbVideoData (searchResult);
            foreach (dynamic video in tmdbJson) {
                string url = GetVideoId (video);
                string type = GetVideoType (video);
                ListOfVideos.Add ((url, type));
            }
        }

        private string GetVideoType (dynamic video) {
            return video.type.ToString ().ToLower ();
        }

        private bool MatchYear<T> (SearchMovie searchResult, T content) where T : IContent {
            if (searchResult.ReleaseDate.Value.Year == content.Year) {
                return true;
            } else {
                return false;
            }
        }

        private string GetVideoId (dynamic video) {
            return video.key;
        }

        private bool IsVideoType (dynamic searchResult, string type) {
            if (searchResult == type.ToLower ()) {
                return true;
            }
            return false;
        }

        private dynamic GetTmdbVideoData (SearchMovie searchResult) {
            dynamic tmdbJson = UtilClass.GetDynamicJson ($"https://api.themoviedb.org/3/movie/{searchResult.Id}/videos?language=en-US&api_key={tmdbClient.ApiKey}");
            return tmdbJson.results;
        }

        private bool MatchTitle<T> (SearchMovie searchMovie, T content) where T : IContent {
            if (UtilClass.CleanMediaName (searchMovie.Title) == UtilClass.CleanMediaName (content.Name)) {
                return true;
            } else { return false; }
        }

        public IDownloader GetDownloader()
        {
            return youtubeDownloader;
        }
    }
}