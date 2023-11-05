namespace PS.Portal.Domain.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int TotalRecords { get; set; }
        public PaginatedList(List<T> source, int pageIndex, int pageSize)
        {
            TotalRecords = source.Count;
            List<T> items;

            if (TotalRecords <= pageSize)
            {
                items = source.Take(pageSize).ToList();
            }
            else if (TotalRecords < pageIndex * pageSize)
            {
                var endPage = (int)Math.Ceiling((decimal)TotalRecords / (decimal)pageSize);
                items = source.Skip((endPage - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }

            this.AddRange(items);
        }
    }
}
