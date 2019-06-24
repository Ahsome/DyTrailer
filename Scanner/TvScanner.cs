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
                Tv tv = new Tv (contentDate.Name, contentDate.Year, DirectoryLocation);
                Trailer trailer = new Trailer (true, true);
                Teaser teaser = new Teaser (true, true);
                BehindTheScene bts = new BehindTheScene (true, true);
                Featurette featurette = new Featurette(true, true);
                tv.AddMedia (trailer);
                tv.AddMedia (teaser);
                tv.AddMedia (bts);
                tv.AddMedia(featurette);
                ListOfContent.Add (tv);
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