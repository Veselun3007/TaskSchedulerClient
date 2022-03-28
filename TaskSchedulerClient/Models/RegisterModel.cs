using System.ComponentModel.DataAnnotations;
using TaskSchedulerClient.Models.Interfaces;

namespace TaskSchedulerClient.Models
{
    /// <summary>
    /// Клас, що описує модель реєстрації
    /// </summary>
    public class RegisterModel : IUser
    {
        public RegisterModel() {}

        [Display(Name = "Логін")]
        [Required(ErrorMessage = "Не вказаний логін")]
        [StringLength(53, MinimumLength = 5,
            ErrorMessage = "Введіть логін довжиною від 5 до 53 символів")]
        public string UserName { get; set; }

        [Display(Name = "Електрона пошта")]
        [Required(ErrorMessage = "Не вказаний Email")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не вказаний пароль")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        [Display(Name = "Підтвердіть пароль пароль")]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Пароль введено невірно")]
        public string ConfirmPassword { get; set; }

        public byte[] UserImage { get; set; }
    }
}
