using System.ComponentModel.DataAnnotations;

namespace POS.Web.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "يرجى إدخال اسم المستخدم")]
        [Display(Name = "اسم المستخدم")]
        public string Username { get; set; }

        [Required(ErrorMessage = "يرجى إدخال كلمة المرور")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; }
    }
}
