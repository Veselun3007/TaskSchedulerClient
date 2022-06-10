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
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "Введіть логін довжиною від 3 до 50 символів")]
        [RegularExpression(@"\A[А-ЯA-ZЇІЄҐЬа-яa-zїієєґь._0-9 ]{3,50}\z", ErrorMessage = "Недопустимі символи")]
        public string UserName { get; set; }

        [Display(Name = "Електрона пошта")]
        [Required(ErrorMessage = "Не вказаний Email")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не вказаний пароль")]
        [RegularExpression(@"^(?=.{10,}$)(?=(?:.*?[A-Z]){2})(?=.*?[a-z])(?=(?:.*?[0-9]){2}).*$",
            ErrorMessage = "Пароль має містити 10 символів та як мінімум 2 великі та 1 " +
            "рядкову літеру латинського алфавіту, 2 цифри")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        [Display(Name = "Підтвердіть пароль пароль")]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Пароль введено невірно")]
        public string ConfirmPassword { get; set; }

        public byte[] UserImage { get; set; }
    }
}
