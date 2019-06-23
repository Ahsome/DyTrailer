using System;
using System.Threading.Tasks;

namespace DyTrailer
{
    public interface IScraper
    {
         void SetTrailerUrl<T>(T trailer) where T:ITrailer;
         void StartDownload<T>(T trailer) where T:ITrailer;
    }
}