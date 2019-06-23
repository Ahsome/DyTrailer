namespace DyTrailer
{
    public interface IDownloader
    {
         void Download<T>(string url, T media) where T : IMedia;
    }
}