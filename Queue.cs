using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DyTrailer
{
    public class Queue
    {
        private List<IContent> listOfContent = new List<IContent>();

        public void AddToQueue<T>(T content) where T : IContent
        {
            listOfContent.Add(content);
        }

        public void AddToQueue(List<IContent> contents)
        {
            listOfContent.AddRange(contents);
        }

        public async Task StartDownload()
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(listOfContent, content =>
                {
                    var listOfScrapers = UtilClass.GetPossibleScrapers();
                    DownloadContentMedias(content, listOfScrapers).Wait();
                });
            });
            Console.WriteLine("DONE: All possible media files completed!");
        }

        private async Task DownloadContentMedias(IContent content, List<IScraper> scrapers)
        {
            foreach (var scraper in scrapers.Where(c => c.SupportedContent.Contains(content.Type)))
            {
                var taskMediaDownloading = new List<Task>();
                //TODO: Make it so SetVideos only adds one video per type please
                scraper.SetPossibleVideos(content);
                foreach (var media in content.Medias.Where(c => scraper.SupportedMedia.Contains(c.Type)))
                {
                    if (!MediaExists(media))
                    {
                        var video = scraper.ListOfVideos.First(c => c.Type.Contains(media.Type));
                        Directory.CreateDirectory(media.FileDirectory);

                        //TODO: Make it so it downloads in temporary location, then moves
                        taskMediaDownloading.Add(scraper.GetDownloader().Download(video.Url, media));
                        Console.WriteLine($"INFO: Started downloading: {media.FileName}");
                    }
                }
                await Task.WhenAll(taskMediaDownloading);
            }
        }

        //TODO: Extract this method to run in scanner. This way it does not need to check for files
        private bool MediaExists<T>(T media) where T : IMedia
        {
            //TODO: Do check for all filetypes
            if (Directory.Exists(media.FileDirectory))
            {
                var possibleMedias = Directory.GetFiles(media.FileDirectory, $"{media.FileName}.*");
                if (possibleMedias.Length > 0)
                {
                    Console.WriteLine($"WARNING: {media.FileName} already exists, so will skip downloading");
                    return true;
                }
            }
            return false;
        }
    }
}