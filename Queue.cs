using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Async;

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

        public async Task StartDownload () {
            var foreachTask = Task.Run(() => {
            Parallel.ForEach (listOfContent, content =>  {
                var listOfScrapers = GetPossibleScrapers ();
                DownloadContentMedias (content, listOfScrapers).Wait();
            });
            });
            await foreachTask;
        }

        private async Task DownloadContentMedias (IContent content, List<IScraper> listOfNewScrapers) {
            List<Task> taskMediaDownloading = new List<Task>();
            foreach (IScraper scraper in listOfNewScrapers) {
                if (!scraper.SupportedContent.Contains (content.Type)) {
                    continue;
                }
                //TODO: Make it so SetVideos only adds one video per type please
                scraper.SetPossibleVideos (content);
                foreach (var media in content.MediaToDownload)
                {
                    taskMediaDownloading.Add(Task.Run(() => DownloadIndividualMedia (media, scraper)));
                }
            }
            await Task.WhenAll(taskMediaDownloading);
        }

        private async Task DownloadIndividualMedia (IMedia media, IScraper scraper) {
            //TODO: only make folder if the supported media exists
            Directory.CreateDirectory (media.FileDirectory);
            if (MediaExists (media) || !scraper.SupportedMedia.Contains (media.Type)) {
                return;
            }
            foreach ((string Url, string Type) video in scraper.ListOfVideos) {
                if (video.Type == media.Type) {
                    var downloader = scraper.GetDownloader();
                    await downloader.Download (video.Url, media);
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