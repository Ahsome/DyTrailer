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
                DirectoryInfo di = new DirectoryInfo (directory);
                string folderName = di.Name;
                (string Name, int Year) contentDate = GetContentData (folderName);
                Tv content = new Tv (contentDate.Name, contentDate.Year, DirectoryLocation);
                Trailer trailer = new Trailer (true, true);
                Teaser teaser = new Teaser (true, true);
                BehindTheScene bts = new BehindTheScene (true, true);
                Featurette featurette = new Featurette (true, true);
                Clip clip = new Clip (true, true);
                Blooper blooper = new Blooper (true, true);
                content.AddMedia (trailer);
                content.AddMedia (teaser);
                content.AddMedia (bts);
                content.AddMedia (featurette);
                content.AddMedia (clip);
                content.AddMedia (blooper);
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