using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskSchedulerClient.Models.Interfaces;
using TaskShedulerDesktopClient.Data.Errors;

namespace TaskSchedulerClient.Models
{

    /// <summary>
    /// Клас, що описує модель аутентифікація
    /// </summary>
    
    public class LoginModel : IUser
    {
        public ErrorInfo ErrorInfo { get; set; } = new ErrorInfo();
        public LoginModel() {}

        [Display(Name = "Логін")]
        [Required(ErrorMessage = "Не вказаний логін")]
        public string UserName { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не вказаний пароль")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

    }
}
