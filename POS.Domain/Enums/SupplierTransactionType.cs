using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Enums
{
    public enum SupplierTransactionType
    {
        PurchaseInvoice = 1, // فاتورة مشتريات
        CashPayment = 2,     // دفع نقدي
        ReturnInvoice = 3,   // مرتجع مشتريات
        OpeningBalance = 4   // رصيد افتتاحي
    }
}
