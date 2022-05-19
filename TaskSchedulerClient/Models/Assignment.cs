using System;
using System.ComponentModel.DataAnnotations;

namespace TaskSchedulerClient.Models
{
    public class Assignment
    {
        
        public int AssignmentId { get; set; }

        [Required(ErrorMessage = "Відсутня назва")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Некоректна довжина назви")]
        public string AssignmentName { get; set; }

        [StringLength(250, ErrorMessage = "Некоректна довжина опису")]
        public string AssignmentDescription { get; set; }

        [Required(ErrorMessage = "Відсутній час")]
        public DateTime? AssignmentTime { get; set; }
        public bool? AssignmentState { get; set; }      
        public int UserId { get; set; }

        public Assignment() { }
    }
}
