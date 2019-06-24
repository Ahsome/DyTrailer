using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DyTrailer
{
    public class Queue {
        List<IContent> listOfContent;

        public Queue () {
            listOfContent = new List<IContent> ();
        }

        public void AddToQueue<T> (T content) where T : IContent {
            listOfContent.Add (content);
        }

        public void AddToQueue (List<IContent> contents) {

            listOfContent.AddRange (contents);
        }

        private List<IScraper> GetPossibleScrapers () {
            var tmdbScraper = new TmdbScraper ();
            var youtubeRentScraper = new YoutubeRentScraper ();
            var appleScraper = new AppleScraper ();
            return new List<IScraper> () { tmdbScraper };
        }

        public void StartDownload () {
            Parallel.ForEach (listOfContent, content => {
                var listOfScrapers = GetPossibleScrapers ();
                DownloadContentsMedia (content, listOfScrapers);
            });
        }

        private void DownloadContentsMedia (IContent content, List<IScraper> listOfNewScrapers) {
            foreach (IScraper scraper in listOfNewScrapers) {
                if (!scraper.SupportedContent.Contains (content.Type)) {
                    continue;
                }
                scraper.SetPossibleVideos (content);
                foreach (var media in content.MediaToDownload)
                {
                    DownloadIndividualMedia (media, scraper);
                }
            }
        }

        private void DownloadIndividualMedia (IMedia media, IScraper scraper) {
            //TODO: only make folder if the supported media exists
            Directory.CreateDirectory (media.FileDirectory);
            if (MediaExists (media) || !scraper.SupportedMedia.Contains (media.Type)) {
                return;
            }
            foreach ((string Url, string Type) video in scraper.ListOfVideos) {
                if (video.Type == media.Type) {
                    var downloader = scraper.GetDownloader();
                    downloader.Download (video.Url, media);
                    //* NOTE: This breaks downloading once one version of the file is downloaded. Consider downloading multiple versions?
                    break;
                }
            }
        }

        private bool MediaExists<T> (T media) where T : IMedia {
            //TODO: Do check for all filetypes
            var possibleMedias = Directory.GetFiles (media.FileDirectory, $"{media.FileName}.*");
            if (possibleMedias.Length > 0) {
                return true;
            }
            return false;
        }
    }

}