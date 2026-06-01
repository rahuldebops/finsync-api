namespace finsyncapi.Models
{
    public class PagedResponse<T>
    {
        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            Page = pageNumber;
            PageSize = pageSize;
            Total = totalRecords;
        }

        public IEnumerable<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }

        /*public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;*/
    }


}
