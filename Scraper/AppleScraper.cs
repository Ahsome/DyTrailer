using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DyTrailer
{
    internal class AppleScraper : IScraper
    {
        public List<(string Url, string Type)> ListOfVideos { get; } = new List<(string Url, string Type)>();
        public List<string> SupportedMedia { get; } = new List<string>() { "trailer", "clip", "featurette" };
        public List<string> SupportedContent { get; } = new List<string>() { "movie" };

        public void SetPossibleVideos<T>(T content) where T : IContent
        {
            string cleanedName = UtilClass.CleanMediaName(content.Name).Replace(' ', '+');
            string appleFindUrl = $"https://trailers.apple.com/trailers/home/scripts/quickfind.php?q={cleanedName}";
            dynamic appleFindJson = UtilClass.GetDynamicJson(appleFindUrl);
            //TODO: Do check to see if the first one is actually the one we want
            try
            {
                string appleDataUrl = $"https://trailers.apple.com{appleFindJson.results[0].location}";

                dynamic filmData = UtilClass.GetDynamicJson($"{appleDataUrl}/data/page.json");
                foreach (dynamic clip in filmData.clips)
                {
                    dynamic videoType = clip.title.ToString().ToLower();
                    //TODO: Change it so videoType actually stores it in the same way as we need it (i.e. trailer, not trailer 2 etc.)
                    if (!ListOfVideos.Any(x => x.Type == videoType))
                    {
                        ListOfVideos.Add((clip.versions.enus.sizes.hd1080.src, videoType));
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"WARNING: {content.Name} does not exist on Apple Trailers. Continuing on");
            }
        }

        public IDownloader GetDownloader()
        {
            return new AppleDownloader();
        }
    }
}