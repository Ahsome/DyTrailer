using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DyTrailer
{
    internal class TvScanner : IScanner
    {
        public TvScanner(string tvDirectory)
        {
            DirectoryLocation = tvDirectory;
            ListOfContent = new List<IContent>();
        }

        public List<IContent> ListOfContent { get; }

        public string DirectoryLocation { get; }

        public async Task ScanFolderAsync()
        {
            Task scanFolderAsync = Task.Run(() => ScanFolder());
            await scanFolderAsync;
        }

        public void ScanFolder()
        {
            Parallel.ForEach(Directory.GetDirectories(DirectoryLocation), directory =>
            {
                (string Name, int Year) = GetContentData(new DirectoryInfo(directory).Name);
                Tv content = new Tv(Name, Year, DirectoryLocation);
                foreach (var media in UtilClass.GetPossibleMedias())
                {
                    content.AddMedia(media);
                }
                ListOfContent.Add(content);
            });
        }

        private (string name, int year) GetContentData(string folderName)
        {
            int leftBracketIndex = folderName.IndexOf('(');
            string name = folderName.Substring(0, leftBracketIndex).Trim().ToLower();
            int year = Convert.ToInt32(folderName.Substring(leftBracketIndex + 1, 4));
            return (name, year);
        }
    }
}