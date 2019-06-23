using System;
using System.IO;

namespace DyTrailer {
    public class Teaser : IMedia  {
        bool shouldAppend;
        bool shouldSeparateFolder;
        public Teaser(bool shouldAppend, bool shouldSeparateFolder )
        {
            this.shouldAppend = shouldAppend;
            this.shouldSeparateFolder = shouldSeparateFolder;
        }
        public string FileName { get; private set; }

        public string FileDirectory { get; private set; }

        public string Type => "teaser";
        void SetFileDirectory<T> (T content) where T : IContent {
            if (shouldSeparateFolder) {
                FileDirectory = Path.Combine (content.ContentDirectory, Type);
            } else {
                FileDirectory = content.ContentDirectory;
            }
        }

        void SetFileName<T> (T content) where T : IContent {
            //TODO: Separate creating name to separate function
            if (shouldAppend) {
                FileName = $"{content.Name} ({content.Year})-{Type}";
            } else {
                FileName = $"{content.Name} ({content.Year})";
            };
        }

        public void SetProperties<T> (T content) where T : IContent {
            SetFileName (content);
            SetFileDirectory (content);
        }
    }
}