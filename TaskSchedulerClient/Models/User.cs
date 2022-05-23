using System;
using System.ComponentModel.DataAnnotations;
using TaskSchedulerClient.Models.Interfaces;

namespace TaskSchedulerClient.Models
{
    /// <summary>
    /// Клас, що описує таблицю User
    /// </summary>
    public class User : IUser
    {
        [Required(ErrorMessage = "Відсутній id")]
        [Range(1, Int32.MaxValue, 
            ErrorMessage = "Недопустиме значення id")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Відсутній логін")]
        [StringLength(50, MinimumLength = 3, 
            ErrorMessage = "Некоректна довжина логіну")]
        public string UserName { get; set; }

        [Required(ErrorMessage = 
            "Відсутній адрес електронної пошти")]
        [EmailAddress(ErrorMessage = 
            "Не відповідає електронній пошті")]
        [StringLength(50, ErrorMessage = 
            "Некоректна довжина електронної пошти")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Відсутній пароль")]
        [StringLength(50, ErrorMessage = 
            "Некоректна довжина паролю")]
        public string UserPassword { get; set; }

        public User() { }

    }
}
