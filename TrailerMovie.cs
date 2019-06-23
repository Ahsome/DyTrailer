using System;
using System.IO;

namespace DyTrailer {
    public class TrailerMovie : ITrailer {
        public TrailerMovie (string name, int year, string fileLocation, bool shouldAppend) {
            Name = name;
            Year = year;

            FileName = findFileName (shouldAppend);
            FileDirectory = findFileDirectory (fileLocation);
        }

        public TrailerMovie (string totalString, string fileLocation, bool shouldAppend) {
            //TODO: Handle implementation so you can have multiple brackets
            //TODO: Could be done by separating every (something) into a list, then checking which ones have only 4 digit numbers
            Name = findName (totalString);
            Year = findYear (totalString);

            FileName = findFileName (shouldAppend);
            FileDirectory = findFileDirectory (fileLocation);
        }

        private int findYear (string totalString) {
            int bracketIndex = totalString.IndexOf ('(')+1;
            return Convert.ToInt32 (totalString.Substring (bracketIndex, 4));
        }

        private string findName (string totalString) {
            int leftBracketIndex = totalString.IndexOf ('(');
            return totalString.Substring (0, leftBracketIndex).Trim();
        }

        private string findFileDirectory (string fileLocation) {
            return Path.Combine (fileLocation, $"{Name} ({Year})");
        }

        private string findFileName (bool shouldAppend) {
            //TODO: Separate creating name to separate function
            if (shouldAppend) {
                return $"{Name} ({Year})-trailer";
            } else {
                return $"{Name} ({Year})";
            }
        }

        public string Name { get; private set; }
        public int Year { get; private set; }
        public string FileName { get; private set; }
        public string FileDirectory { get; private set; }
        public string Type => "Movie";
    }
}