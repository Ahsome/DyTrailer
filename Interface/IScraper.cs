using System.Collections.Generic;

namespace DyTrailer
{
    public interface IScraper {
        void SetPossibleVideos<T> (T media) where T : IContent;

        List < (string Url, string Type) > ListOfVideos { get; }

        IDownloader GetDownloader();
    }
}