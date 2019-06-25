using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DyTrailer {
    internal class MovieScanner : IScanner {
        public MovieScanner (string moviesDirectory) {
            DirectoryLocation = moviesDirectory;
            ListOfContent = new List<IContent> ();
        }

        public List<IContent> ListOfContent { get; private set; }

        public string DirectoryLocation { get; private set; }

        public async Task ScanFolderAsync() {
            Task scanFolderAsync = Task.Run(() => ScanFolder());
            await scanFolderAsync;
        }

        public void ScanFolder () {
            Parallel.ForEach (Directory.GetDirectories (DirectoryLocation), directory => {
                (string Name, int Year) contentData = GetContentData (new DirectoryInfo(directory).Name);
                Movie content = new Movie (contentData.Name, contentData.Year, DirectoryLocation);
                foreach (var media in UtilClass.GetPossibleMedias())
                {
                    content.AddMedia(media);
                }
                ListOfContent.Add (content);
            });
        }

        private (string name, int year) GetContentData (string folderName) {
            int leftBracketIndex = folderName.IndexOf ('(');
            string name = folderName.Substring (0, leftBracketIndex).Trim ();
            int year = Convert.ToInt32 (folderName.Substring (leftBracketIndex + 1, 4));
            return (name, year);
        }
    }
}