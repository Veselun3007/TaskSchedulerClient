using System;
using System.ComponentModel.DataAnnotations;

namespace TaskSchedulerClient.Models
{
    public class Assignment
    {
        
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public string AssignmentDescription { get; set; }
        public DateTime? AssignmentTime { get; set; }
        public bool? AssignmentState { get; set; }      
        public int UserId { get; set; }

        public Assignment() { }
    }
}
