﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MoneyFox.Core.Interfaces;
using MoneyFox.Core.SettingAccess;
using MoneyFox.Core.Resources;
using PropertyChanged;

namespace MoneyFox.Core.ViewModels
{
    [ImplementPropertyChanged]
    public class SettingsSecurityViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly IPasswordStorage passwordStorage;

        public SettingsSecurityViewModel(IPasswordStorage passwordStorage, IDialogService dialogService)
        {
            this.passwordStorage = passwordStorage;
            this.dialogService = dialogService;
        }

        /// <summary>
        ///     Grants the GUI access to the password setting.
        /// </summary>
        public bool IsPasswortActive
        {
            get { return Settings.PasswordRequired; }
            set
            {
                Settings.PasswordRequired = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     The password who the user set.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     The password confirmation the user entered.
        /// </summary>
        public string PasswordConfirmation { get; set; }

        /// <summary>
        ///     Save the password to the secure storage of the current platform
        /// </summary>
        public RelayCommand SavePasswordCommand => new RelayCommand(SavePassword);

        /// <summary>
        ///     Loads the password from the secure storage
        /// </summary>
        public RelayCommand LoadCommand => new RelayCommand(LoadData);

        /// <summary>
        ///     Remove the password from the secure storage
        /// </summary>
        public RelayCommand UnloadCommand => new RelayCommand(RemovePassword);

        private void SavePassword()
        {
            if (Password != PasswordConfirmation)
            {
                dialogService.ShowMessage(Strings.PasswordConfirmationWrongTitle,
                    Strings.PasswordConfirmationWrongMessage);
                return;
            }

            passwordStorage.SavePassword(Password);

            dialogService.ShowMessage(Strings.PasswordSavedTitle, Strings.PasswordSavedMessage);
        }

        private void LoadData()
        {
            if (IsPasswortActive)
            {
                Password = passwordStorage.LoadPassword();
                PasswordConfirmation = passwordStorage.LoadPassword();
            }
        }

        private void RemovePassword()
        {
            if (!IsPasswortActive)
            {
                passwordStorage.RemovePassword();
            }

            //  Deactivate option again if no password was entered
            if (IsPasswortActive && string.IsNullOrEmpty(Password))
            {
                IsPasswortActive = false;
            }
        }
    }
}