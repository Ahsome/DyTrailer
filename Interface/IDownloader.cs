using System.Threading.Tasks;

namespace DyTrailer
{
    public interface IDownloader
    {
         Task Download<T>(string url, T media) where T : IMedia;
    }
}