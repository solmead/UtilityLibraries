using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Extensions
{

    public enum SortOrderEnum
    {
        A2Z,
        Z2A
    }


    public class PagingFilter
	{
		public PagingFilter()
		{
			ItemsPerPage = 20;
		}
		public int ItemsPerPage { get; set; }
		public int Page { get; set; }
        public SortOrderEnum SortOrder { get; set; }
        public int Total { get; set; }
        public int Pages { get; set; }
    }

	public class PagingList<tt> 
	{
		public PagingList(IQueryable<tt> query, PagingFilter filter, Action<List<tt>> onList = null )
        {
            Filter = filter;
            filter.Total = query.Count();
			if (filter.ItemsPerPage <= 0)
			{
				filter.ItemsPerPage = filter.Total;
			}
			var start = filter.Page * filter.ItemsPerPage;
			if (start >= filter.Total)
			{
				start = filter.Total;
			}
			var cnt = filter.ItemsPerPage;
			if (start + cnt >= filter.Total)
			{
				cnt = filter.Total - start;
			}
            filter.Pages = (int)((filter.Total * 1.0) / (filter.ItemsPerPage * 1.0) );
		    if (filter.Pages < 0)
		    {
                filter.Pages = 0;
		    }
            if (filter.Total == 0)
            {
                List = new List<tt>();
                return;
            }
            if ((filter.ItemsPerPage % filter.Total) != 0)
		    {
                filter.Pages++;
		    }
            List = query.Skip(start).Take(cnt).ToList();
		    if (onList != null)
		    {
                onList(List);
		    }
		} 

		public List<tt> List { get; set; }

		public PagingFilter Filter { get; set; } 


	}

}
