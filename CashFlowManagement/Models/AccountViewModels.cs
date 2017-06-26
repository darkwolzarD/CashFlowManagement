﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CashFlowManagement.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Nhớ mật khẩu")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "{0} phải dài tối thiểu {2} kí tự.", MinimumLength = 6)]
        [Display(Name = "Tên tài khoản")]
        public string Username { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} phải dài tối thiểu {2} kí tự.", MinimumLength = 6)]
        [Display(Name = "Tên người dùng")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} phải dài tối thiểu {2} kí tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không giống nhau")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Giới tính")]
        public int Sex { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "{0} phải dài tối thiểu {2} kí tự.", MinimumLength = 1)]
        [Display(Name = "Công việc")]
        public string Job { get; set; }

        [Required]
        [Display(Name = "Số con")]
        public int NumberOfChildren { get; set; }
    }
}
