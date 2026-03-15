using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Enums
{
    public enum TransactionType
    {
        Sale = 1,     
        Payment = 2,   
        Expense = 3,
        ReturnSale = 4,
        Income = 5
    }
}
