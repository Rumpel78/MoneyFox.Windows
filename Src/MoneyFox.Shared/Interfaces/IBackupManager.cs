using System;
using System.Threading.Tasks;

namespace MoneyFox.Shared.Interfaces
{
    /// <summary>
    ///     Defines the interface for a BackupManager who handles the different functions of a backup.
    /// </summary>
    public interface IBackupManager
    {
        /// <summary>
        ///     Logs the user in to the backup service
        /// </summary>
        Task Login();

        /// <summary>
        ///     Checks if there are backups to restore.
        /// </summary>
        /// <returns>Backups available or not.</returns>
        Task<bool> IsBackupExisting();

        /// <summary>
        ///     Returns the date when the last backup was created.
        /// </summary>
        /// <returns>Creation date of the last backup.</returns>
        Task<DateTime> GetBackupDate();

        /// <summary>
        ///     Creates a new backup.
        /// </summary>
        Task CreateNewBackup();

        /// <summary>
        ///     Restores an existing backup.
        /// </summary>
        Task RestoreBackup();
    }
}