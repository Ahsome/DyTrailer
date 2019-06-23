using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DyTrailer {
    public class Queue {
        List<IScraper> listOfScrapers;
        List<IContent> listOfContent;

        public Queue () {
            SetPossibleScrappers ();
            listOfContent = new List<IContent> ();
        }

        public void AddToQueue<T> (T content) where T : IContent {
            listOfContent.Add (content);
        }

        public void AddToQueue (List<IContent> contents) {

            listOfContent.AddRange (contents);
        }

        private void SetPossibleScrappers () {
            var tmdbScraper = new TmdbScraper ();
            var youtubeRentScraper = new YoutubeRentScraper ();
            var appleScraper = new AppleScraper ();
            listOfScrapers = new List<IScraper> () { tmdbScraper };
        }

        public void DownloadMedia() {
            foreach (IContent content in listOfContent) {
                foreach (IScraper scraper in listOfScrapers) {
                    scraper.SetPossibleVideos (content);
                    IDownloader downloader = scraper.GetDownloader ();
                    foreach (IMedia media in content.MediaToDownload) {
                        Directory.CreateDirectory (media.FileDirectory);
                        if (MediaExists (media)) {
                            return;
                        }
                        foreach ((string Url, string Type) video in scraper.ListOfVideos) {
                            if (video.Type == media.Type) {
                                downloader.Download (video.Url, media);
                            }
                        }
                    }
                }
            }

            /*/
            foreach (IMedia media in content.MediaToDownload) {
                Directory.CreateDirectory (media.FileDirectory);
                foreach (IScraper scraper in listOfScrapers) {
                    if (MediaExists (media)) {
                        return;
                    }
                    scraper.SetPossibleVideos (media);
                    var downloader = scraper.ReturnDownloader ();
                    foreach ((string Url, string Type) video in scraper.ListOfVideos) {
                        if (video.Type == media.Type) {
                            if (MediaExists (media)) {
                                return;
                            }
                            downloader.Download (video.Url, media);
                        }
                    }

                }
            }
            */
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