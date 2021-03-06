﻿using MoneyFox.Foundation.Model;

namespace MoneyManager.Foundation.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        void AddPaymentAmount(Payment payment);

        void RemovePaymentAmount(Payment payment);

        void RemovePaymentAmount(Payment payment, Account account);
    }
}