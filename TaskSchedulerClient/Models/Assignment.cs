using System;

namespace TaskSchedulerAPI.Models
{
    public class Assignment
    {
        public int AssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public string AssignmentDescription { get; set; }
        public DateTime AssignmentTime { get; set; }
        public bool? AssignmentState { get; set; }
        public int UserID { get; set; }

        public Assignment() { }
    }
}
