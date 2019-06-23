using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NYoutubeDL;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Models.MediaStreams;

namespace DyTrailer {
    public class YoutubeDownloader : IDownloader {
        public void Download<T> (string url, T media) where T : IMedia {
            var converter = new YoutubeConverter ();
            var videoId = url;

            if (url.Contains ("youtu")) {
                videoId = YoutubeClient.ParseVideoId (url);
            }
            converter.DownloadVideoAsync ($"{videoId}", Path.Combine (media.FileDirectory, $"{media.FileName}.mp4")).Wait ();
        }
    }
}