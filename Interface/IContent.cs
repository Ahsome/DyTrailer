using System.Collections.Generic;

namespace DyTrailer {
    public interface IContent {
        string Name { get; }
        int Year { get; }
        string ContentDirectory { get; }
        string Type {get; }
        List<IMedia> Medias { get; }
    }
}