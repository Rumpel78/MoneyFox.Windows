﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MoneyFox.Foundation.Interfaces;
using MoneyFox.Foundation.Model;
using PropertyChanged;
using SQLite.Net;

namespace MoneyFox.DataAccess
{
    /// <summary>
    ///     Handles the access to the Payments table on the database
    /// </summary>
    [ImplementPropertyChanged]
    public class PaymentDataAccess : AbstractDataAccess<Payment>
    {
        private readonly ISqliteConnectionFactory connectionFactory;

        public PaymentDataAccess(ISqliteConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        /// <summary>
        ///     Saves a new item or updates an existing
        /// </summary>
        /// <param name="itemToSave">Item to SaveItem</param>
        protected override void SaveToDb(Payment itemToSave)
        {
            using (var db = connectionFactory.GetConnection())
            {
                SaveRecurringPayment(itemToSave, db);
                //Don't use insert or replace here, because it will always replace the first element
                if (itemToSave.Id == 0)
                {
                    db.Insert(itemToSave);
                    itemToSave.Id = db.Table<Payment>().OrderByDescending(x => x.Id).First().Id;
                }
                else
                {
                    db.Update(itemToSave);
                }
            }
        }

        private void SaveRecurringPayment(Payment payment, SQLiteConnection db)
        {
            if (payment.IsRecurring)
            {
                //Don't use insert or replace here, because it will always replace the first element
                if (payment.RecurringPaymentId == 0)
                {
                    db.Insert(payment.RecurringPayment);
                    payment.RecurringPayment = db.Table<RecurringPayment>().OrderByDescending(x => x.Id).First();
                }
                else
                {
                    db.Update(payment.RecurringPayment);
                }
            }
        }

        /// <summary>
        ///     Deletes an item from the database
        /// </summary>
        /// <param name="payment">Item to Delete.</param>
        protected override void DeleteFromDatabase(Payment payment)
        {
            using (var dbConn = connectionFactory.GetConnection())
            {
                dbConn.Delete(payment);
            }
        }

        /// <summary>
        ///     Loads a list of payments from the database filtered by the expression
        /// </summary>
        /// <param name="filter">filter expression.</param>
        /// <returns>List of loaded payments.</returns>
        protected override List<Payment> GetListFromDb(Expression<Func<Payment, bool>> filter)
        {
            using (var db = connectionFactory.GetConnection())
            {
                var listQuery = db.Table<Payment>();

                if (filter != null)
                {
                    listQuery = listQuery.Where(filter);
                }

                return listQuery.ToList();
            }
        }
    }
}