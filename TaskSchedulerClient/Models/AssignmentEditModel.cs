using System;
using System.ComponentModel.DataAnnotations;
using TaskSchedulerAPI.Models;

namespace TaskSchedulerClient.Models
{
    public class AssignmentEditModel
    {

        public int AssignmentId { get; set; }

        
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Некоректна довжина назви")]
        public string AssignmentName { get; set; }

        [StringLength(250, ErrorMessage = "Некоректна довжина опису")]
        public string AssignmentDescription { get; set; }

        public DateTime? AssignmentTime { get; set; }

        public bool? AssignmentState { get; set; }

        public int UserId { get; set; }

        public static explicit operator AssignmentEditModel(Assignment obj)
        {
            return new AssignmentEditModel()
            {
                AssignmentId = obj.AssignmentId,
                AssignmentName = obj.AssignmentName,
                AssignmentDescription = obj.AssignmentDescription,
                AssignmentTime = obj.AssignmentTime,
                AssignmentState = obj?.AssignmentState,
                UserId = obj.UserId,
            };
        }

    }
}
