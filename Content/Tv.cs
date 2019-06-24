using System;
using System.Collections.Generic;
using System.IO;

namespace DyTrailer
{
    public class Tv : IContent {
        public Tv (string name, int year, string mediaFolder) {
            Name = name;
            Year = year;
            SetContentDirectory(mediaFolder);
        }

        public Tv (string totalString, string mediaFolder) {
            //TODO: Handle implementation so you can have multiple brackets
            //TODO: Could be done by separating every (something) into a list, then checking which ones have only 4 digit numbers
            Name = findName (totalString);
            Year = findYear (totalString);
            SetContentDirectory(mediaFolder);
        }

        private void SetContentDirectory(string mediaFolder)
        {
            ContentDirectory = Path.Combine(mediaFolder, $"{Name} ({Year})");
        }

        private int findYear (string totalString) {
            int bracketIndex = totalString.IndexOf ('(') + 1;
            return Convert.ToInt32 (totalString.Substring (bracketIndex, 4));
        }

        private string findName (string totalString) {
            int leftBracketIndex = totalString.IndexOf ('(');
            return totalString.Substring (0, leftBracketIndex).Trim ();
        }

        public void AddMedia (IMedia media) {
            MediaToDownload.Add (media);
            media.SetProperties(this);
        }
        public string Name { get; private set; }
        public int Year { get; private set; }
        public string ContentDirectory { get; private set; }
        public List<IMedia> MediaToDownload { get; private set; } = new List<IMedia> ();
        public string Type => "tv";
    }
}