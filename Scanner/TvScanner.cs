using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DyTrailer {
    internal class TvScanner : IScanner {
        public TvScanner (string tvDirectory) {
            DirectoryLocation = tvDirectory;
            ListOfContent = new List<IContent>();
        }

        public List<IContent> ListOfContent { get; private set; }

        public string DirectoryLocation { get; private set; }

        public void ScanFolder () {
            Parallel.ForEach (Directory.GetDirectories (DirectoryLocation), directory => {
                (string Name, int Year) contentDate = GetContentData (new DirectoryInfo(directory).Name);
                Tv content = new Tv (contentDate.Name, contentDate.Year, DirectoryLocation);
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