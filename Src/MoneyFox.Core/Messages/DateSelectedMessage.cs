﻿using System;

namespace MoneyFox.Core.Messages
{
    public class DateSelectedMessage
    {
        public DateSelectedMessage(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}