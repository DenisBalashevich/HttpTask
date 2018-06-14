using System;
using System.IO;

namespace HttpTask
{
    public class SiteSaver : ISiteSaver
    {
        private readonly DirectoryInfo _rootDirectory;
        private readonly ILogger _logger;

        public SiteSaver(string directoryPath,
            ILogger logger)
        {
            _rootDirectory = new DirectoryInfo(directoryPath);
            _logger = logger;
        }

        public void SaveHtmlDocument(Uri uri, string name, Stream documentStream)
        {
            var directoryPath = CombineLocations(_rootDirectory, uri);
            Directory.CreateDirectory(directoryPath);
            var filePath = Path.Combine(directoryPath, name);

            SaveToFile(documentStream, filePath);
            documentStream.Close();
            _logger.Log($"Html document {uri} saved in {filePath}");
        }

        private static string CombineLocations(FileSystemInfo directory, Uri uri)
        {
            return Path.Combine(directory.FullName, uri.Host) + uri.LocalPath.Replace("/", @"\");
        }

        private static void SaveToFile(Stream stream, string fileFullPath)
        {
            var createdFileStream = File.Create(fileFullPath);
            stream.CopyTo(createdFileStream);
            createdFileStream.Close();
        }
    }
}
