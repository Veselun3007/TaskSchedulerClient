using System;
using System.ComponentModel.DataAnnotations;

namespace TaskSchedulerClient.Models
{
    public class Assignment
    {
        
        public int AssignmentId { get; set; }

        [Required(ErrorMessage = "Відсутня назва")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Некоректна довжина назви")]
        [RegularExpression(@"\A[А-ЯA-ZЇІЄҐЬа-яa-zїієєґь._0-9 ]{1,50}\z", ErrorMessage = "Недопустимі символи")]
        public string AssignmentName { get; set; }

        [StringLength(250, ErrorMessage = "Некоректна довжина опису")]
        public string AssignmentDescription { get; set; }

        [Required(ErrorMessage = "Відсутній час")]
        [DisplayFormat(DataFormatString = "{0:dd':'MM':'yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? AssignmentTime { get; set; }
        public bool? AssignmentState { get; set; }      
        public int UserId { get; set; }

        public Assignment() { }
    }
}
