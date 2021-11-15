namespace Core.Paging
{
    public class PageParameter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PageParameter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public PageParameter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 1 : pageSize;
        }
    }
}
