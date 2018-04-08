﻿namespace YouTube.Downloader
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using Caliburn.Micro;

    using YouTube.Downloader.Factories;
    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Services;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.Utilities;
    using YouTube.Downloader.Utilities.Interfaces;
    using YouTube.Downloader.ViewModels;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class AppBootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        internal AppBootstrapper()
        {
            Initialize();
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            // Register Services
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.Singleton<IAppDataService, AppDataService>();
            _container.Singleton<IDownloadService, DownloadService>();
            _container.Singleton<ISettingsService, SettingsService>();
            _container.Singleton<IYouTubeApiService, YouTubeApiService>();

            _container.Singleton<IFileSystemUtility, FileSystemUtility>();

            // Register Factories
            _container.Singleton<IDownloadFactory, DownloadFactory>();
            _container.Singleton<IYouTubeFactory, YouTubeFactory>();

            // Register ViewModels
            _container.Singleton<IShellViewModel, ShellViewModel>();
            _container.Singleton<IMainViewModel, MainViewModel>();

            _container.Singleton<IQueryViewModel, QueryViewModel>();
            _container.Singleton<IMatchedVideosViewModel, MatchedVideosViewModel>();
            _container.PerRequest<IMatchedVideoViewModel, MatchedVideoViewModel>();

            _container.Singleton<ICurrentDownloadsViewModel, CurrentDownloadsViewModel>();
            _container.PerRequest<IDownloadViewModel, DownloadViewModel>();

            _container.PerRequest<IVideoViewModel, VideoViewModel>();

            _container.Singleton<ISettingsViewModel, SettingsViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShellViewModel>();
        }
    }
}