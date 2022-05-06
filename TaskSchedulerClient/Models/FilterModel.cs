using System;

namespace TaskSchedulerClient.Models
{
    public class FilterModel
    {
        public FilterModel(DateTime? startDate, DateTime? endDate, string name)
        {
            StartDate = startDate;
            EndDate = endDate;
            SelectedName = name;
        }
        public string SelectedName { get; private set; }  
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

    }
}
