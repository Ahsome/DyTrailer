using System;
using System.Collections.Generic;
using System.IO;

namespace DyTrailer {
    public class TrailerQueue {
        Boolean shouldAppend;
        YoutubeRentScraper youtubeRentScraper;
        TmdbScraper tmdbScraper;
        List<IScraper> listOfScrapers;

        public TrailerQueue () {
            var tmdbScraper = new TmdbScraper();
            var youtubeRentScraper = new YoutubeRentScraper ();
            var appleScraper = new AppleScraper();
            listOfScrapers = new List<IScraper> () { appleScraper};
        }

        public void DownloadTrailer<T> (T trailer) where T : ITrailer {
            Directory.CreateDirectory (trailer.FileDirectory);
            foreach (IScraper scraper in listOfScrapers) {
                if (trailerExists (trailer)) {
                    return;
                }
                scraper.SetTrailerUrl (trailer);
                scraper.StartDownload(trailer);
            }

        }

        private bool trailerExists<T> (T trailer) where T : ITrailer {
            //TODO: Do check for all filetypes
            if (File.Exists (Path.Combine (trailer.FileDirectory, $"{trailer.FileName}.mp4"))) {
                return true;
            }
            return false;
        }
    }

}