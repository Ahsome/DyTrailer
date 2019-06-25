using System.Collections.Generic;
using System.Threading.Tasks;

namespace DyTrailer
{
    public interface IScanner
    {
        List<IContent> ListOfContent {get; }
        string DirectoryLocation {get; }
        void ScanFolder();
        Task ScanFolderAsync();
    }
}