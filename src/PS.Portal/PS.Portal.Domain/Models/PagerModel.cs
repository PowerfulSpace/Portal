using Microsoft.AspNetCore.Mvc.Rendering;

namespace PS.Portal.Domain.Models
{
    public class PagerModel
    {

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public int StartRecord { get; private set; }
        public int EndRecord { get; private set; }

        public string Action { get; set; } = "Index";
        public string SearchText { get; set; } = null!;
        public string SortExpression { get; set; } = null!;

        public PagerModel(int totalItems, int currentPage, int pageSize)
        {
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;

            TotalPages = (int)Math.Ceiling((decimal)TotalItems / (decimal)pageSize);

            var startPage = currentPage - 5;
            var endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > TotalPages)
            {
                endPage = TotalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            //Альтернативная логика
            //StartRecord = (pageSize * currentPage) -(pageSize - 1);
            //EndRecord = pageSize * currentPage;

            StartRecord = (CurrentPage - 1) * PageSize + 1;
            EndRecord = StartRecord - 1 + PageSize;

            if (EndRecord > TotalItems)
            {
                EndRecord = TotalItems;
            }
            if (TotalItems == 0)
            {
                StartRecord = 0;
                EndRecord = 0;
                StartPage = 0;
                CurrentPage = 0;
            }
            else
            {
                StartPage = startPage;
                EndPage = endPage;

                if (totalItems <= pageSize)
                {
                    StartRecord = 1;
                    EndRecord = totalItems;
                    StartPage = 1;
                    CurrentPage = 1;
                }
                if (totalItems < currentPage * pageSize)
                {
                    StartRecord = (EndPage * pageSize) - (pageSize - 1);
                    CurrentPage = EndPage;
                }
            }
        }
        public List<SelectListItem> GetPageSizes()
        {
            List<SelectListItem> pageSizes = new List<SelectListItem>();

            if (2 == PageSize)
                pageSizes.Add(new SelectListItem(2.ToString(), 2.ToString(), true));
            else
                pageSizes.Add(new SelectListItem(2.ToString(), 2.ToString()));


            for (int i = 5; i <= 50; i += 5)
            {
                if (i == PageSize)
                {
                    pageSizes.Add(new SelectListItem(i.ToString(), i.ToString(), true));
                }
                else
                {
                    pageSizes.Add(new SelectListItem(i.ToString(), i.ToString()));
                }
            }

            return pageSizes;
        }
    }
}
