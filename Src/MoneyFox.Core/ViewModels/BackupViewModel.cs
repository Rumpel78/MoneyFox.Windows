﻿using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.ApplicationInsights;
using Microsoft.OneDrive.Sdk;
using MoneyFox.Core.Exceptions;
using MoneyFox.Core.Interfaces;
using MoneyFox.Core.Resources;

namespace MoneyManager.Core.ViewModels
{
    public class BackupViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly IRepositoryManager repositoryManager;

        public BackupViewModel(IRepositoryManager repositoryManager,
            IBackupService backupService,
            IDialogService dialogService)
        {
            this.repositoryManager = repositoryManager;
            this.dialogService = dialogService;

            BackupService = backupService;
        }

        /// <summary>
        ///     The Date when the backup was modified the last time.
        /// </summary>
        public DateTime BackupLastModified { get; private set; }

        /// <summary>
        ///     Prepares the View when loaded.
        /// </summary>
        public RelayCommand LoadedCommand => new RelayCommand(Loaded);

        /// <summary>
        ///     The Backup Service for the current platform.
        ///     On Android this needs to be overwritten with an
        ///     instance with the current activity setup.
        /// </summary>
        public IBackupService BackupService { get; }

        /// <summary>
        ///     Will create a backup of the database and upload it to onedrive
        /// </summary>
        public RelayCommand BackupCommand => new RelayCommand(CreateBackup);

        /// <summary>
        ///     Will download the database backup from onedrive and overwrite the
        ///     local database with the downloaded.
        ///     All datamodels are then reloaded.
        /// </summary>
        public RelayCommand RestoreCommand => new RelayCommand(RestoreBackup);

        /// <summary>
        ///     Indicator if something is in work.
        /// </summary>
        public bool IsLoading { get; private set; }

        private async void Loaded()
        {
            BackupLastModified = await BackupService.GetBackupDate();
        }

        private async void CreateBackup()
        {
            await Login();

            if (!await ShowOverwriteBackupInfo())
            {
                return;
            }

            IsLoading = true;
            await BackupService.Upload();
            await ShowCompletionNote();
            IsLoading = false;
        }

        private async void RestoreBackup()
        {
            await Login();

            if (!await ShowOverwriteDataInfo())
            {
                return;
            }

            IsLoading = true;

            await BackupService.Restore();
            repositoryManager.ReloadData();

            await ShowCompletionNote();
            IsLoading = false;
        }

        private async Task Login()
        {
            try
            {
                IsLoading = true;
                await BackupService.Login();
                IsLoading = false;
            }
            catch (ConnectionException)
            {
                await dialogService.ShowMessage(Strings.LoginFailedTitle, Strings.LoginFailedMessage);
            }
            catch (OneDriveException ex)
            {
                new TelemetryClient().TrackException(ex);
                await dialogService.ShowMessage(Strings.LoginFailedTitle, Strings.LoginFailedMessage);
            }
        }

        private async Task<bool> ShowOverwriteBackupInfo()
        {
            return await dialogService
                .ShowConfirmMessage(Strings.OverwriteTitle, Strings.OverwriteBackupMessage);
        }

        private async Task<bool> ShowOverwriteDataInfo()
        {
            return await dialogService
                .ShowConfirmMessage(Strings.OverwriteTitle, Strings.OverwriteDataMessage);
        }

        private async Task ShowCompletionNote()
        {
            await dialogService.ShowMessage(Strings.SuccessTitle, Strings.TaskSuccessfulMessage);
        }
    }
}