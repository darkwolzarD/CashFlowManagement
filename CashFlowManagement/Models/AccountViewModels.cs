using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CashFlowManagement.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Địa chỉ email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Nhớ mật khẩu")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nhập tên người dùng")]
        [StringLength(50, ErrorMessage = "{0} phải dài tối thiểu {2} kí tự.", MinimumLength = 6)]
        [Display(Name = "Tên người dùng")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Nhập email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "{0} phải dài tối thiểu {2} kí tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không giống nhau")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Chọn giới tính")]
        [Display(Name = "Giới tính")]
        public int Sex { get; set; }

        [Required(ErrorMessage = "Nhập công việc")]
        [StringLength(20, ErrorMessage = "{0} phải dài tối thiểu {2} kí tự.", MinimumLength = 1)]
        [Display(Name = "Công việc")]
        public string Job { get; set; }

        [Required(ErrorMessage = "Nhập số con")]
        [Display(Name = "Số con")]
        public int NumberOfChildren { get; set; }
    }
}
