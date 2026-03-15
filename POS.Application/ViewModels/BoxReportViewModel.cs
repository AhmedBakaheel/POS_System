using System.ComponentModel.DataAnnotations;

namespace POS.Application.ViewModels
{
    public class BoxReportViewModel
    {
        public int ShiftId { get; set; }

        [Display(Name = "اسم الموظف")]
        public string EmployeeName { get; set; }

        [Display(Name = "تاريخ الوردية")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date { get; set; }

        [Display(Name = "الرصيد الافتتاحي")]
        public decimal StartingCash { get; set; }

        [Display(Name = "المتوقع")]
        public decimal ExpectedCash { get; set; }

        [Display(Name = "الفعلي")]
        public decimal ActualCash { get; set; }

        [Display(Name = "الفرق")]
        public decimal Difference => ActualCash - ExpectedCash;
    }
}