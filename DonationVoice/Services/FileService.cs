using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var invalidChars = string.Join("", Path.GetInvalidFileNameChars());
            var wordArray = Regex.Split(input, @"\P{L}").Take(10);
            var snakeCase = string.Join("-", wordArray).ToLower();
            var cleansed = Regex.Replace(snakeCase, @$"[{invalidChars}]", "");

            return cleansed += ".ogg";
        }

        async public Task SaveVoice(string filePath, string url)
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
