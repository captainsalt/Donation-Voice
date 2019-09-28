using DonationVoice.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DonationVoice.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly HttpClientWrapper _client;
        private readonly FileService _fileService;
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

        public MainWindowViewModel(HttpClientWrapper client, FileService fileService)
        {
            _client = client;
            _fileService = fileService;
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

        private void ClearTextCommand() => Input = null;

        private void OpenSaveDirectoryCommand() => Process.Start("explorer.exe", _saveDirectory);

        private async void GetVoiceCommand()
        {
            if (SelectedVoice == null || string.IsNullOrWhiteSpace(Input)) return;

            Url = await _client.GetVoiceUrl(Input, SelectedVoice);

            var fileName = _fileService.GenerateFileNameFromString(Input);
            FullFilePath = Path.Combine(_saveDirectory, SelectedVoice, fileName);
            await _fileService.SaveVoice(FullFilePath, Url);

            Prompt = $"Saved to {FullFilePath}";
        }
    }
}
