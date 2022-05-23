namespace TaskSchedulerClient.Models
{
    public class SortingModel
    {
        public SortingState NameSortAsc { get; private set; } 
        public SortingState NameSortDesc { get; private set; } 
        public SortingState DateSortAsc { get; private set; }  
        public SortingState DateSortDesc { get; private set; }         
        public SortingState StateSortAsc { get; private set; }   
        public SortingState StateSortDesc { get; private set; } 
        public SortingState Current { get; private set; }     

        public SortingModel(SortingState sortOrder)
        {
            NameSortAsc = sortOrder == SortingState.NameAsc ? SortingState.NameDesc : SortingState.NameAsc;
            NameSortDesc= sortOrder == SortingState.NameDesc ? SortingState.NameAsc : SortingState.NameDesc;
            DateSortAsc = sortOrder == SortingState.DateAsc ? SortingState.DateDesc : SortingState.DateAsc;
            DateSortDesc = sortOrder == SortingState.DateDesc ? SortingState.DateAsc : SortingState.DateDesc;
            StateSortAsc = sortOrder == SortingState.StateAsc ? SortingState.StateDesc : SortingState.StateAsc;
            StateSortDesc = sortOrder == SortingState.StateDesc ? SortingState.StateAsc : SortingState.StateDesc;
            Current = sortOrder;
        }

        #region *** Enum ***
        public enum SortingState
        {
            NameAsc,    
            NameDesc,  
            DateAsc, 
            DateDesc,   
            StateAsc, 
            StateDesc 
        }
        #endregion

    }
}
