namespace Application.Common.Helpers
{
    public class PaginationHeader
    {
        #region constructor
        public PaginationHeader(int currentPage, int itemsPerPage,int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
        }
        #endregion


        #region Properties
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }     
        public int TotalItems { get; set; }

        #endregion

    }
}