﻿using System.Threading.Tasks;
using Microsoft.OneDrive.Sdk;
using MoneyFox.Core.Constants;
using MoneyFox.Core.Interfaces;

namespace MoneyFox.Core.Services
{
    public class OneDriveAuthenticator : IOneDriveAuthenticator
    {
        private readonly IDialogService dialogService;

        private IOneDriveClient oneDriveClient;

        public OneDriveAuthenticator(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public async Task<IOneDriveClient> LoginAsync()
        {
            try
            {
                if (oneDriveClient == null)
                {
                    oneDriveClient = OneDriveClientExtensions.GetUniversalClient(OneDriveConstants.Scopes);
                    await oneDriveClient.AuthenticateAsync();
                }

                if (!oneDriveClient.IsAuthenticated)
                {
                    await oneDriveClient.AuthenticateAsync();
                }

                return oneDriveClient;
            }
            catch (OneDriveException exception)
            {
                // Swallow authentication cancelled exceptions
                if (!exception.IsMatch(OneDriveErrorCode.AuthenticationCancelled.ToString()))
                {
                    if (exception.IsMatch(OneDriveErrorCode.AuthenticationFailure.ToString()))
                    {
                        await dialogService.ShowMessage(
                            "Authentication failed",
                            "Authentication failed");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return oneDriveClient;
        }
    }
}