using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodaySales { get; set; }       
        public int TodayPurchasesCount { get; set; }   
        public int LowStockCount { get; set; }      
        public decimal BoxBalance { get; set; }        
    }
}
