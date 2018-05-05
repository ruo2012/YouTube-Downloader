﻿namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class CurrentDownloadsViewModel : ViewModelBase, ICurrentDownloadsViewModel, IHandle<IEnumerable<IVideoViewModel>>
    {
        private readonly IDownloadService _downloadService;

        private readonly IDownloadFactory _downloadFactory;

        public CurrentDownloadsViewModel(IEventAggregator eventAggregator, IDownloadFactory downloadFactory, IVideoFactory videoFactory, IDataService dataService, IDownloadService downloadService)
        {
            eventAggregator.Subscribe(this);
            _downloadService = downloadService;
            _downloadFactory = downloadFactory;

            AddDownloads(dataService.Load<YouTubeVideo>("Downloads").Select(videoFactory.MakeVideoViewModel));

            ((ListCollectionView)CollectionViewSource.GetDefaultView(Downloads)).CustomSort = Comparer<IDownloadViewModel>.Create((first, second) => -first.DownloadStatus.DownloadState.CompareTo(second.DownloadStatus.DownloadState));
        }

        public IObservableCollection<IDownloadViewModel> Downloads { get; } = new BindableCollection<IDownloadViewModel>();

        public IObservableCollection<IDownloadViewModel> SelectedDownloads { get; } = new BindableCollection<IDownloadViewModel>();

        public void Handle(IEnumerable<IVideoViewModel> message)
        {
            AddDownloads(message);
        }

        public void TogglePause()
        {
            SelectedDownloads.Apply(download => download.Download.TogglePause());
        }

        public void Kill()
        {
            SelectedDownloads.Apply(download => download.Download.Kill());
        }

        private void AddDownloads(IEnumerable<IVideoViewModel> videos)
        {
            IDownloadViewModel[] newDownloads = videos.Select(_downloadFactory.MakeDownloadViewModel).ToArray();

            foreach (IDownloadViewModel download in newDownloads)
            {
                void DownloadCompleted(object sender, EventArgs e)
                {
                    download.Download.Exited -= DownloadCompleted;
                    download.DownloadStatus.PropertyChanged -= DownloadStatusPropertyChanged;

                    Task.Delay(3_000).ContinueWith(task =>
                    {
                        Downloads.Remove(download);
                        SelectedDownloads.Remove(download);
                    });
                }

                download.Download.Exited += DownloadCompleted;

                void DownloadStatusPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == nameof(DownloadStatus.DownloadState))
                    {
                        Downloads.Refresh();
                    }
                }

                download.DownloadStatus.PropertyChanged += DownloadStatusPropertyChanged;
            }

            Downloads.AddRange(newDownloads);
            _downloadService.QueueDownloads(newDownloads.Select(download => download.Download));
        }
    }
}