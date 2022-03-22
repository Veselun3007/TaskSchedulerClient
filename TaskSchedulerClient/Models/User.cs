using TaskSchedulerClient.Models.Interfaces;

namespace TaskSchedulerClient.Models
{
    /// <summary>
    /// Клас, що описує таблицю User
    /// </summary>
    public class User : IUser
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserPassword { get; set; }

        public string UserImage { get; set; }

    }
}
