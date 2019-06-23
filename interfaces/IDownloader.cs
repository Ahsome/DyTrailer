namespace DyTrailer
{
    public interface IDownloader
    {
         void Download<T>(string url, T trailer) where T : ITrailer;
    }
}