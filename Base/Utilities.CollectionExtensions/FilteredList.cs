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
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public string SortColumn { get; set; }
        public OrderDirectionEnum? OrderDirection { get; set; }

        //page, rows, sidx, sord
        //public int page { get { return Page; } set { Page = value; } }
        public int? rows { get { return PageSize; } set { PageSize = value; } }
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

        private IQueryable<tt> BaseData { get; set; }


        public delegate IQueryable<tt> QueryableFunction(int? page, int? pageSize, string sortColumn, FilteredInfo.OrderDirectionEnum? orderDirection);

        private QueryableFunction getFilteredData = null;


        private Func<int> getItemCount = null;



        public FilteredInfo Info { get; set; }

        public int TotalItems => getItemCount();
        public int TotalPages => (int)Math.Ceiling(((double)TotalItems) /((double)(Info.PageSize ?? 20)));

        public FilteredList(FilteredInfo info, QueryableFunction queryableFunction, Func<int> getTotalItemCount)
        {
            Info = info;
            getFilteredData = queryableFunction;
            getItemCount = getTotalItemCount;
        }
        public FilteredList(IQueryable<tt> baseData, FilteredInfo info)
        {
            Info = info;
            BaseData = baseData;
            //Info.PageSize = 25;
            //Info.Page = 1;


            getFilteredData = (int? page, int? pageSize, string sortColumn, FilteredInfo.OrderDirectionEnum? orderDirection) =>
            {
                return GetFilteredDataFromBase(page, pageSize, sortColumn, orderDirection);
            };
            getItemCount = () =>
            {
                return BaseData.Count();
            };

        }
        public IQueryable<tt> GetFilteredData()
        {
            return getFilteredData(Info.Page, Info.PageSize, Info.SortColumn, Info.OrderDirection);
        }


        private IQueryable<tt> GetFilteredDataFromBase(int? page, int? pageSize, string sortColumn, FilteredInfo.OrderDirectionEnum? orderDirection)
        {
            if (page > TotalPages)
            {
                page = TotalPages;
            }
            if (page < 1)
            {
                page = 1;
            }
            if (TotalItems == 0)
            {
                return BaseData;
            }
            int startPosition;

            try
            {
                startPosition = checked(((page ?? 1) - 1) * (pageSize ?? 20));
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

            int stopPosition = startPosition + (pageSize ?? 20);
            if (stopPosition > TotalItems)
            {
                stopPosition = TotalItems;
            }

            IQueryable<tt> pList = BaseData;

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                try
                {
                    pList = BaseData.OrderBy(sortColumn + " " + orderDirection?.ToString());
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