﻿using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MoneyFox.Core.Constants;
using MoneyFox.Core.DatabaseModels;
using MoneyFox.Core.Interfaces;
using MoneyFox.Core.Interfaces.ViewModels;
using MoneyFox.Core.Resources;
using MoneyManager.Core.ViewModels;
using PropertyChanged;
using IDialogService = MoneyFox.Core.Interfaces.IDialogService;

namespace MoneyFox.Core.ViewModels
{
    [ImplementPropertyChanged]
    public class AccountListViewModel : ViewModelBase
    {
        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;

        public AccountListViewModel(IAccountRepository accountRepository,
            IPaymentRepository paymentRepository,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            this.accountRepository = accountRepository;
            this.dialogService = dialogService;
            this.navigationService = navigationService;

            BalanceViewModel = new BalanceViewModel(accountRepository, paymentRepository);
        }

        public IBalanceViewModel BalanceViewModel { get; }

        /// <summary>
        ///     All existing accounts.
        /// </summary>
        public ObservableCollection<Account> AllAccounts
        {
            get { return accountRepository.Data; }
            set { accountRepository.Data = value; }
        }

        /// <summary>
        ///     Prepares the account list
        /// </summary>
        public RelayCommand LoadedCommand => new RelayCommand(Loaded);

        /// <summary>
        ///     Open the PaymentViewModel overview for this account.
        /// </summary>
        public RelayCommand<Account> OpenOverviewCommand => new RelayCommand<Account>(GoToPaymentViewModelOverView);

        /// <summary>
        ///     Edit the selected account
        /// </summary>
        public RelayCommand<Account> EditAccountCommand => new RelayCommand<Account>(EditAccount);

        /// <summary>
        ///     Deletes the selected account
        /// </summary>
        public RelayCommand<Account> DeleteAccountCommand => new RelayCommand<Account>(Delete);

        /// <summary>
        ///     Prepare everything and navigate to AddAccount view
        /// </summary>
        public RelayCommand GoToAddAccountCommand => new RelayCommand(GoToAddAccount);

        private void EditAccount(Account account)
        {
            navigationService.NavigateTo(NavigationConstants.MODIFY_ACCOUNT_VIEW, account);
        }

        private void Loaded()
        {
            BalanceViewModel.UpdateBalanceCommand.Execute(null);
        }

        private void GoToPaymentViewModelOverView(Account account)
        {
            if (account == null)
            {
                return;
            }

            accountRepository.Selected = account;
            navigationService.NavigateTo(NavigationConstants.PAYMENT_LIST_VIEW);
        }

        private async void Delete(Account item)
        {
            if (item == null)
            {
                return;
            }

            if (await dialogService.ShowConfirmMessage(Strings.DeleteTitle, Strings.DeleteAccountConfirmationMessage))
            {
                accountRepository.Delete(item);
            }
        }

        private void GoToAddAccount()
        {
            navigationService.NavigateTo(NavigationConstants.MODIFY_ACCOUNT_VIEW);
        }
    }
}