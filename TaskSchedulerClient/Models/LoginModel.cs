using System.ComponentModel.DataAnnotations;
using TaskSchedulerClient.Models.Interfaces;

namespace TaskSchedulerClient.Models
{
    /// <summary>
    /// Клас, що описує модель аутентифікація
    /// </summary>
    public class LoginModel : IUser
    {
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
