using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DonationVoice.Services
{
    public class FileService
    {
        private readonly HttpClientWrapper _client;

        public FileService(HttpClientWrapper client)
        {
            _client = client;
        }

        public string GenerateFileNameFromString(string input)
        {
            var cleansed = Regex.Replace(input, @$"[^\w\s]", "");
            var snakeCase = Regex.Replace(cleansed, @"\s+", "-").Trim();

            return snakeCase += ".ogg";
        }

        public async Task SaveVoice(string filePath, string url)
        {
            var fileDirectory = new FileInfo(filePath).DirectoryName;

            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);

            using var fs = File.Create(filePath);
            var stream = await _client.GetStream(url);
            await stream.CopyToAsync(fs);
        }
    }
}
