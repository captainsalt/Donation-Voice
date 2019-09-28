using DonationVoice.Services;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DonationVoice.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly HttpClientWrapper _client;
        private readonly string _saveDirectory;
        private DelegateCommand _getVoiceCommand;
        private DelegateCommand _openSaveDirectoryCommand;
        private DelegateCommand _clearText;
        private string _url;
        private string _fullFilePath;
        private string _input;
        private string _selectedVoice;
        private string _prompt;

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        public string FullFilePath
        {
            get => _fullFilePath;
            set => SetProperty(ref _fullFilePath, value);
        }

        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        public string SelectedVoice
        {
            get => _selectedVoice;
            set => SetProperty(ref _selectedVoice, value);
        }

        public string Prompt
        {
            get => _prompt;
            set => SetProperty(ref _prompt, value);
        }

        public IReadOnlyList<string> VoiceList { get; }

        public DelegateCommand ClearText =>
            _clearText ?? (_clearText = new DelegateCommand(ClearTextCommand));

        public DelegateCommand GetVoice =>
            _getVoiceCommand ?? (_getVoiceCommand = new DelegateCommand(GetVoiceCommand));

        public DelegateCommand OpenSaveDirectory =>
            _openSaveDirectoryCommand ?? (_openSaveDirectoryCommand = new DelegateCommand(OpenSaveDirectoryCommand));

        public MainWindowViewModel(HttpClientWrapper client)
        {
            _client = client;
            _saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Voice Lines");
            VoiceList = new List<string>()
            {
                "Brian",
                "Ivy",
                "Justin",
                "Russell",
                "Nicole",
                "Emma",
                "Amy",
                "Joanna",
                "Salli",
                "Kimberly",
                "Kendra",
                "Joey",
                "Mizuki",
                "Chantal",
                "Mathieu",
                "Maxim",
                "Raveena"
            };
        }

        void ClearTextCommand() => Input = null;

        void OpenSaveDirectoryCommand()
        {
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = _saveDirectory
            };

            fileDialog.ShowDialog();
        }

        private async void GetVoiceCommand()
        {
            if (SelectedVoice == null || string.IsNullOrWhiteSpace(Input))
            {
                return;
            }

            var nameFragments = Regex.Split(Input, @"\s+").Take(15);
            var snakeCaseFile = string.Join("-", nameFragments);

            Url = await _client.GetVoiceUrl(Input, SelectedVoice);

            if (nameFragments.Count() == 15)
            {
                snakeCaseFile += "...";
            }

            snakeCaseFile += ".ogg";
            FullFilePath = Path.Combine(_saveDirectory, SelectedVoice, snakeCaseFile);

            var fileDirectory = new FileInfo(FullFilePath).DirectoryName;

            if (!Directory.Exists(fileDirectory))
            {
                new DirectoryInfo(fileDirectory).Create();
            }

            using var fs = File.Create(FullFilePath);
            var stream = await _client.GetStream(Url);
            await stream.CopyToAsync(fs);

            Prompt = $"Saved to {FullFilePath}";
        }
    }
}
