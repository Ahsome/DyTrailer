using System;
using System.IO;

namespace DyTrailer {
    class MainProgram {
        static void Main (string[] args) {
            //TODO: Rather than use DownloadMovie, maybe have AddMovie so to download in Queue
            TrailerQueue trailerDownloader = new TrailerQueue ();

            //TODO: Use this code to get name and year out of string:
            TrailerMovie trailer = new TrailerMovie ("Avengers Infinity War (2018)", Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Movies"), true);
            trailerDownloader.DownloadTrailer (trailer);
        }
    }
}