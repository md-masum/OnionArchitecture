using System;
using Core.Common;

namespace Core.Paging
{
    public class PagedResponse<T> : ApiResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalItem { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize, int totalItem)
        {
            TotalItem = totalItem;
            TotalPage = (int)Math.Ceiling(totalItem / (double)pageSize);
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = data;
            Message = null;
            IsSuccess = true;
            Errors = null;
        }
    }
}
