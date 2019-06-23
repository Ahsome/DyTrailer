using System;
using System.Collections.Generic;
using System.IO;

namespace DyTrailer {
    internal class MovieScanner : IScanner {
        public MovieScanner (string moviesDirectory) {
            DirectoryLocation = moviesDirectory;
            ListOfContent = new List<IContent>();
        }

        public List<IContent> ListOfContent { get; private set; }

        public string DirectoryLocation { get; private set; }

        public void ScanFolder () {
            foreach (string directory in Directory.GetDirectories (DirectoryLocation)) {
                DirectoryInfo di = new DirectoryInfo (directory);
                string folderName = di.Name;
                (string Name, int Year) contentDate = GetContentData (folderName);
                Movie movie = new Movie (contentDate.Name, contentDate.Year, DirectoryLocation);
                Trailer trailer = new Trailer(true,true);
                movie.AddMedia(trailer);
                ListOfContent.Add(movie);
            }
        }

        private (string name, int year) GetContentData (string folderName) {
            int leftBracketIndex = folderName.IndexOf ('(');
            string name = folderName.Substring (0, leftBracketIndex).Trim ();
            int year = Convert.ToInt32 (folderName.Substring (leftBracketIndex + 1, 4));
            return (name, year);
        }
    }
}