using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DyTrailer {
    class MainProgram {
        static void Main (string[] args) {
            string newPath = AppDomain.CurrentDomain.BaseDirectory;
            string movieDirectory = Path.Combine (newPath, "Movies");
            string tvDirectory = Path.Combine (newPath, "TV Shows");

            var movieScanner = new MovieScanner (movieDirectory);
            movieScanner.ScanFolder ();

            var tvScanner = new TvScanner (tvDirectory);
            tvScanner.ScanFolder ();

            //TODO: Rather than use DownloadMovie, maybe have AddMovie so to download in Queue
            var queue = new Queue ();
            //TODO: Make it that we do not need to add to queue since content should already contain all the movies and trailers
            queue.AddToQueue (movieScanner.ListOfContent);
            queue.AddToQueue (tvScanner.ListOfContent);

            queue.StartDownload ();
        }
    }
}