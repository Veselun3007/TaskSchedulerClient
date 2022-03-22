using System;
using TaskSchedulerAPI.Models;

namespace TaskSchedulerClient.Models
{
    public class AssignmentEditModel
    {

        public int AssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public string AssignmentDescription { get; set; }
        public DateTime AssignmentTime { get; set; }
        public bool? AssignmentState { get; set; }
        public int UserID { get; set; }

        public static explicit operator
         AssignmentEditModel(Assignment obj)
        {
            return new AssignmentEditModel()
            {
                AssignmentID = obj.AssignmentID,
                AssignmentName = obj.AssignmentName,
                AssignmentDescription = obj.AssignmentDescription,
                AssignmentTime = obj.AssignmentTime,
                AssignmentState = obj.AssignmentState,
                UserID = obj.UserID,
            };
        }

    }
}
