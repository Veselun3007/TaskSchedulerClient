using System.Collections.Generic;

namespace TaskSchedulerClient.Models
{
    public class IndexModel
    {
        public User Users { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }
        public SortingModel SortViewModel { get; set; }
        public FilterModel FilterViewModel { get; set; }
    }
}
