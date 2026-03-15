using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Enums
{
    public enum RoleGroup
    {
        [Display(Name = "إدارة النظام")] SYSTEM_MANAGEMENT = 1,
        [Display(Name = "المبيعات")] SALES = 2,
        [Display(Name = "العملاء")] CUSTOMERS = 3,
        [Display(Name = "المخزن")] STOCK = 4,
        [Display(Name = "التقارير")] REPORTS = 5,
        [Display(Name = "الصندوق ")] BOX_OFFICE = 6,
        [Display(Name = "المستخدمين")] USERS = 7
    }
}
