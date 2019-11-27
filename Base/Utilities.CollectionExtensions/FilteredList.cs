using System;
using System.Linq;
using System.Linq.Dynamic;

namespace System.Collections.Extensions
{
    public class FilteredInfo
    {
        public enum OrderDirectionEnum
        {
            Asc,
            Desc
        }

        public FilteredInfo()
        {
            Page = 1;
            PageSize = 20;
            SortColumn = "";
            //OrderDirection = "ASC";
        }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public string SortColumn { get; set; }
        public OrderDirectionEnum OrderDirection { get; set; }

        //page, rows, sidx, sord
        //public int page { get { return Page; } set { Page = value; } }
        public int rows { get { return PageSize; } set { PageSize = value; } }
        public string sidx { get { return SortColumn; } set { SortColumn = value; } }
        public string sord { get { return OrderDirection.ToString().ToUpper(); }
            set
            {
                if (value == "asc")
                {
                    OrderDirection = OrderDirectionEnum.Asc;
                }
                else
                {
                    OrderDirection = OrderDirectionEnum.Desc;
                }
            }
        }
    }

    public class FilteredList<tt> where tt:class
    {

        public IQueryable<tt> BaseData { get; set; }
        public FilteredInfo Info { get; set; }

        public int TotalItems => BaseData.Count();
        public int TotalPages => (int)Math.Ceiling(((double)BaseData.Count())/((double)Info.PageSize));

        public FilteredList(IQueryable<tt> baseData, FilteredInfo info)
        {
            Info = info;
            BaseData = baseData;
            //Info.PageSize = 25;
            //Info.Page = 1;
        }

        public IQueryable<tt> GetFilteredData()
        {
            if (Info.Page > TotalPages)
            {
                Info.Page = TotalPages;
            }
            if (Info.Page < 1)
            {
                Info.Page = 1;
            }
            if (TotalItems == 0)
            {
                return BaseData;
            }
            int startPosition;

            try
            {
                startPosition = checked((Info.Page - 1) * Info.PageSize);
            }
            catch (OverflowException)
            {
                startPosition = 0;
            }
            if (startPosition >= TotalItems)
            {
                startPosition = TotalItems - 1;
            }
            if (startPosition < 0)
            {
                startPosition = 0;
            }

            int stopPosition = startPosition + Info.PageSize;
            if (stopPosition > TotalItems)
            {
                stopPosition = TotalItems;
            }

            IQueryable<tt> pList = BaseData;

            if (!string.IsNullOrWhiteSpace(Info.SortColumn))
            {
                try
                {
                    pList = BaseData.OrderBy(Info.SortColumn + " " + Info.OrderDirection.ToString());
                }
                catch
                {
                    
                }
            }
            
            if (startPosition < TotalItems && stopPosition <= TotalItems && stopPosition != startPosition)
            {
                pList = pList.Skip(startPosition).Take((stopPosition - startPosition)).ToList().AsQueryable();
            }
            return pList;
        } 


    }
}