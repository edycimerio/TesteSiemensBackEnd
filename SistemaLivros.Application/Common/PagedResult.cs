using System;
using System.Collections.Generic;

namespace SistemaLivros.Application.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult()
        {
            Items = new List<T>();
        }

        public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);
        }
    }
}
