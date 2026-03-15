using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Enums
{
    public enum CustomerTransactionType
    {
        OpeningBalance, // رصيد افتتاحي
        SaleInvoice,    // فاتورة مبيعات
        ReturnInvoice,  // مرتجع مبيعات
        Payment,        // سند قبض (تحصيل نقدى)
        Discount        // خصم مسموح به
    }
}
