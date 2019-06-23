namespace DyTrailer {
    public interface IMedia {
        string FileName { get;}
        string FileDirectory { get;}
        string Type { get; }
        void SetProperties<T>(T content) where T:IContent;
    }
}