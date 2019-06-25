using System;
using System.IO;
using System.Threading.Tasks;

namespace DyTrailer
{
    static class MainProgram
    {
        static async Task Main(string[] args)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string movieDirectory = Path.Combine(currentDirectory, "Movies");
            string tvDirectory = Path.Combine(currentDirectory, "TV Shows");

            var movieScanner = new MovieScanner(movieDirectory);
            var tvScanner = new TvScanner(tvDirectory);

            await Task.WhenAll(movieScanner.ScanFolderAsync(), tvScanner.ScanFolderAsync()).ConfigureAwait(false);

            //TODO: Rather than use DownloadMovie, maybe have AddMovie so to download in Queue
            var queue = new Queue();

            //TODO: Make it that we do not need to add to queue since content should already contain all the movies and trailers
            queue.AddToQueue(movieScanner.ListOfContent);
            queue.AddToQueue(tvScanner.ListOfContent);

            await queue.StartDownload().ConfigureAwait(false);
        }
    }
}