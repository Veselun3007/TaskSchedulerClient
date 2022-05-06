using TaskSchedulerClient.Models.Interfaces;

namespace TaskSchedulerClient.Models
{

    public class UserEditModel : IUser
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserPassword { get; set; }

    }
}
