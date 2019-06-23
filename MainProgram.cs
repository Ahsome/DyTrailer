using System;
using System.IO;

namespace DyTrailer {
    class MainProgram {
        static void Main (string[] args) {
            string movieDirectory = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Movies");
            MovieScanner scanner = new MovieScanner(movieDirectory);
            scanner.ScanFolder();
            //TODO: Rather than use DownloadMovie, maybe have AddMovie so to download in Queue
            Queue queue = new Queue ();
            queue.AddToQueue(scanner.ListOfContent);

            //TODO: Use this code to get name and year out of string:
            //Movie movie = new Movie ("Avengers Infinity War (2018)", movieDirectory);

            queue.DownloadMedia ();
        }
    }
}