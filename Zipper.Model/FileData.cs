namespace Zipper.Model
{

    public class FileData
    {
        ///<exception cref = "ArgumentNullException"></exception>
        public FileData(string name, byte[] bytes, string contentType = "")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (!bytes.Any())
                throw new ArgumentNullException(nameof(bytes));

            Name = name;
            Bytes = bytes;
            ContentType = contentType;
        }

        public string? ContentType { get; set; }

        public string Name { get; set; }

        public byte[] Bytes { get; set; }
    }
}
