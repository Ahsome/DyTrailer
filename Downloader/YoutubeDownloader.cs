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
            await converter.DownloadVideoAsync ($"{videoId}", Path.Combine (media.FileDirectory, $"{media.FileName}.mp4"));
        }
    }
}