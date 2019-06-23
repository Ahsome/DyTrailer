using System.Collections.Generic;

namespace DyTrailer
{
    public interface IScanner
    {
        List<IContent> ListOfContent {get; }
        string DirectoryLocation {get; }
        void ScanFolder();
    }
}