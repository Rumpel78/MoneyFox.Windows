﻿using System;
using System.Collections.Generic;
using System.Linq;
using MoneyFox.Core.Interfaces;
using MoneyFox.Core.Resources;
using MoneyFox.Core.Statistics.Models;
using MoneyFox.Core.ViewModels.Models;

namespace MoneyFox.Core.Statistics.DataProviders
{
    public class CashFlowDataProvider : IStatisticProvider<CashFlow>
    {
        private readonly IPaymentRepository paymentRepository;

        public CashFlowDataProvider(IPaymentRepository paymentRepository)
        {
            this.paymentRepository = paymentRepository;
        }

        /// <summary>
        ///     Selects payments from the given timeframe and calculates the income, the spendings and the revenue.
        ///     There is no differentiation between the accounts, therefore transfers are ignored.
        /// </summary>
        /// <param name="startDate">Startpoint form which to select data.</param>
        /// <param name="endDate">Endpoint form which to select data.</param>
        /// <returns>Statistic value for the given timeframe</returns>
        public CashFlow GetValues(DateTime startDate, DateTime endDate)
        {
            var getPaymentListFunc =
                new Func<List<PaymentViewModel>>(() =>
                    paymentRepository.Data
                        .Where(x => x.Type != PaymentType.Transfer)
                        .Where(x => x.Date >= startDate.Date && x.Date <= endDate.Date)
                        .ToList());

            return GetCashFlowStatisticItems(getPaymentListFunc);
        }

        private CashFlow GetCashFlowStatisticItems(
            Func<List<PaymentViewModel>> getPaymentListFunc)
        {
            var payments = getPaymentListFunc();

            var income = new StatisticItem
            {
                Category = Strings.RevenueLabel,
                Value = payments.Where(x => x.Type == PaymentType.Income).Sum(x => x.Amount)
            };
            income.Label = income.Category + ": " +
                           Math.Round(income.Value, 2, MidpointRounding.AwayFromZero).ToString("C");

            var spent = new StatisticItem
            {
                Category = Strings.ExpenseLabel,
                Value = payments.Where(x => x.Type == PaymentType.Expense).Sum(x => x.Amount)
            };
            spent.Label = spent.Category + ": " +
                          Math.Round(spent.Value, 2, MidpointRounding.AwayFromZero).ToString("C");

            var increased = new StatisticItem
            {
                Category = Strings.IncreaseLabel,
                Value = income.Value - spent.Value
            };
            increased.Label = increased.Category + ": " +
                              Math.Round(increased.Value, 2, MidpointRounding.AwayFromZero).ToString("C");

            return new CashFlow
            {
                Income = income,
                Spending = spent,
                Revenue = increased
            };
        }
    }
}