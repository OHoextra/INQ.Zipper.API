namespace Zipper.Model
{
    public class UnzipRequest
    {
        public Stream? ZipStream { get; set; }

        public string OutPath { get; set; } = "";
    }
}

