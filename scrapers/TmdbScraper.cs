using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace DyTrailer {
    public class TmdbScraper : IScraper {
        TMDbClient tmdbClient;
        YoutubeDownloader youtubeDownloader;
        string url = "";
        public TmdbScraper () {
            //TODO: Insert ApiKey
            tmdbClient = new TMDbClient ("11c74be7cd64fd121c64222bc9e8c27d");
            youtubeDownloader = new YoutubeDownloader ();
        }
        public void SetTrailerUrl<T> (T trailer) where T : ITrailer {
            url = GetTmdbLink (trailer);
        }

        private string GetTmdbLink<T> (T trailer) where T : ITrailer {
            var searchResults = tmdbClient.SearchMovieAsync (trailer.Name).Result;
            foreach (SearchMovie searchResult in searchResults.Results) {
                if (searchResult.ReleaseDate.Value.Year == trailer.Year && matchTitle (searchResult.Title, trailer.Name)) {
                    if (isVideoTrailerType (searchResult)) {
                        return getVideoTrailerId(searchResult);
                    }
                }
            }
            return "";
        }

        public void StartDownload<T>(T trailer) where T:ITrailer
        {
            YoutubeDownloader youtubeDownloader = new YoutubeDownloader();
            youtubeDownloader.Download(url, trailer);
        }

        private string getVideoTrailerId(SearchMovie searchResult)
        {
            dynamic responseSplit = GetTmdbVideoData(searchResult);
            return responseSplit.key;
        }

        private bool isVideoTrailerType (SearchMovie searchResult)
        {
            dynamic responseSplit = GetTmdbVideoData(searchResult);
            if (responseSplit.type ==  "Trailer")
                {
                    return true;
                }
            return false;
        }

        private dynamic GetTmdbVideoData(SearchMovie searchResult)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{searchResult.Id}/videos?language=en-US&api_key={tmdbClient.ApiKey}");
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);

            string response = client.Execute(request).Content;
            dynamic responseJson = JObject.Parse(response);

            return responseJson.results[0];
        }

        private bool matchTitle (string titleOne, string titleTwo) {
            if (UtilClass.CleanTrailerName(titleOne) == UtilClass.CleanTrailerName(titleTwo)) {
                return true;
            } else { return false; }
        }
    }
}