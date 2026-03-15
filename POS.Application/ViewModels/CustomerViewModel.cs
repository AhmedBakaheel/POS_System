using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "اسم العميل مطلوب")]
        public string Name { get; set; }
        public string Phone { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
