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

        //TODO: Set it so its possible supported media changes based on type of content
        public List<string> SupportedMedia {get; } = new List<string>(){"trailer","teaser","clip","featurette","behind the scenes","bloopers"};
        public List<string> SupportedContent {get; } = new List<string>(){"movie","tv"};

        public TmdbScraper () {
            //TODO: Insert ApiKey
            tmdbClient = new TMDbClient ("11c74be7cd64fd121c64222bc9e8c27d");
            ListOfVideos = new List < (string, string) > ();
        }

        public void SetPossibleVideos<T> (T content) where T : IContent {
            ListOfVideos.Clear ();
            switch (content.Type)
            {
                case "movie":
                    FindMovieVideos(content);
                    break;
                case "tv":
                    FindTvVideos(content);
                    break;
            }
        }

        private void FindMovieVideos<T> (T content) where T : IContent {
            var searchResults = tmdbClient.SearchMovieAsync (content.Name).Result;
            foreach (SearchMovie searchResult in searchResults.Results) {
                if (MatchYear (searchResult, content) && MatchTitle (searchResult, content)) {
                    StoreTmdbVideos (searchResult);
                }
            }
        }

        private void FindTvVideos<T> (T content) where T : IContent {
            var searchResults = tmdbClient.SearchTvShowAsync (content.Name).Result;
            foreach (SearchTv searchResult in searchResults.Results) {
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

        private void StoreTmdbVideos (SearchTv searchResult) {
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
        private string GetVideoId (dynamic video) {
            return video.key;
        }

        private bool MatchYear<T> (SearchMovie searchResult, T content) where T : IContent {
            if (searchResult.ReleaseDate.Value.Year == content.Year) {
                return true;
            } else {
                return false;
            }
        }

        private bool MatchYear<T> (SearchTv searchResult, T content) where T : IContent {
            if (searchResult.FirstAirDate.Value.Year == content.Year) {
                return true;
            } else {
                return false;
            }
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

        private dynamic GetTmdbVideoData (SearchTv searchResult) {
            dynamic tmdbJson = UtilClass.GetDynamicJson ($"https://api.themoviedb.org/3/tv/{searchResult.Id}/videos?api_key={tmdbClient.ApiKey}&language=en-US");
            return tmdbJson.results;
        }

        private bool MatchTitle<T> (SearchMovie searchMovie, T content) where T : IContent {
            if (UtilClass.CleanMediaName (searchMovie.Title) == UtilClass.CleanMediaName (content.Name)) {
                return true;
            } else { return false; }
        }

        private bool MatchTitle<T> (SearchTv searchMovie, T content) where T : IContent {
            if (UtilClass.CleanMediaName (searchMovie.Name) == UtilClass.CleanMediaName (content.Name)) {
                return true;
            } else { return false; }
        }

        public IDownloader GetDownloader()
        {
            return new YoutubeDownloader();
        }
    }
}