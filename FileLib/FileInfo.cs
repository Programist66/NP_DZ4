namespace FileLib
{
    public class FileInfo
    {
        public string Name { get; set; }
        public Stream stream {  get; set; }

        public FileInfo(string name, Stream stream)
        {
            Name = name;
            this.stream = stream;
        }
    }
}
