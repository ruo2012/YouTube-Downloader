﻿namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class DownloadFactory : IDownloadFactory
    {
        public IDownloadViewModel MakeDownloadViewModel(IYouTubeVideoViewModel youTubeVideoViewModel)
        {
            IDownloadViewModel downloadViewModel = IoC.Get<IDownloadViewModel>();
            downloadViewModel.Initialise(youTubeVideoViewModel);

            return downloadViewModel;
        }
    }
}