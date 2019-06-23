namespace DyTrailer {
    public interface ITrailer {
        string Name { get; }
        int Year { get; }
        string FileName{get; }
        string FileDirectory{get;}
        string Type{get;}
    }
}