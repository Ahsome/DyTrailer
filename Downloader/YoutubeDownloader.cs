using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace DyTrailer {
    public class YoutubeDownloader : IDownloader {
        public async Task Download<T> (string url, T media) where T : IMedia {
            var converter = new YoutubeConverter ();
            var videoId = url;

            if (url.Contains ("youtu")) {
                videoId = YoutubeClient.ParseVideoId (url);
            }
            try {
            await converter.DownloadVideoAsync ($"{videoId}", Path.Combine (media.FileDirectory, $"{media.FileName}.mp4"));
            } catch (Exception e)
            {
                Console.WriteLine($"Error exception. Message: {e.Message}");
            }
            Console.WriteLine($"COMPLETED: Finished downloading {media.FileName}");
        }
    }
}