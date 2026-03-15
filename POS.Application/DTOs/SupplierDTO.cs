using System.ComponentModel.DataAnnotations;

namespace POS.Application.DTOs
{
    public class SupplierDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المورد مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم طويل جداً")]
        public string Name { get; set; }

        [Display(Name = "الشخص المسؤول")]
        public string? ContactName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public decimal Balance { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}