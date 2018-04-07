﻿namespace YouTube.Downloader.ViewModels
{
    using System;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        public event EventHandler DownloadCompleted;

        public IYouTubeVideoViewModel YouTubeVideoViewModel { get; private set; }

        private DownloadState _downloadState = DownloadState.Waiting;
        public DownloadState DownloadState
        {
            get => _downloadState;

            set
            {
                if (_downloadState == value) return;

                _downloadState = value;
                NotifyOfPropertyChange(() => DownloadState);

                if (_downloadState == DownloadState.Completed)
                {
                    DownloadCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Initialise(IYouTubeVideoViewModel youTubeVideoViewModel)
        {
            YouTubeVideoViewModel = youTubeVideoViewModel;
        }
    }
}