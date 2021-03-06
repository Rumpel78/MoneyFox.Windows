﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MoneyFox.Foundation.Interfaces;
using MoneyFox.Foundation.Model;
using PropertyChanged;

namespace MoneyFox.DataAccess
{
    [ImplementPropertyChanged]
    public class AccountDataAccess : AbstractDataAccess<Account>
    {
        private readonly ISqliteConnectionFactory connectionCreator;

        public AccountDataAccess(ISqliteConnectionFactory connectionCreator)
        {
            this.connectionCreator = connectionCreator;
        }


        /// <summary>
        ///     Saves a Account to the database.
        /// </summary>
        /// <param name="itemToSave">Account to save.</param>
        protected override void SaveToDb(Account itemToSave)
        {
            using (var db = connectionCreator.GetConnection())
            {
                //Don't use insert or replace here, because it will always replace the first element
                if (itemToSave.Id == 0)
                {
                    db.Insert(itemToSave);
                    itemToSave.Id = db.Table<Account>().OrderByDescending(x => x.Id).First().Id;
                }
                else
                {
                    db.Update(itemToSave);
                }
            }
        }

        /// <summary>
        ///     Deletes an Account from the database.
        /// </summary>
        /// <param name="payment">Account to delete</param>
        protected override void DeleteFromDatabase(Account payment)
        {
            using (var db = connectionCreator.GetConnection())
            {
                db.Delete(payment);
            }
        }

        /// <summary>
        ///     Loads an list of accounts from the database filtered by the filter expression.
        /// </summary>
        /// <param name="filter">filter expression</param>
        /// <returns>List of loaded accounts.</returns>
        protected override List<Account> GetListFromDb(Expression<Func<Account, bool>> filter)
        {
            using (var db = connectionCreator.GetConnection())
            {
                var listQuery = db.Table<Account>();

                if (filter != null)
                {
                    listQuery = listQuery.Where(filter);
                }

                return listQuery.OrderBy(x => x.Name).ToList();
            }
        }
    }
}