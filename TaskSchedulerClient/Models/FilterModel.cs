using System;

namespace TaskSchedulerClient.Models
{
    public class FilterModel
    {
        public FilterModel(string startDate, string endDate, string name)
        {
            StartDate = startDate;
            EndDate = endDate;
            SelectedName = name;
        }
        public string SelectedName { get; private set; }  
        public string StartDate { get; private set; }
        public string EndDate { get; private set; }

    }
}
