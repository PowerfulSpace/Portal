using PS.Portal.Domain.Enums;

namespace PS.Portal.Domain.Models
{
    public class SortModel
    {
        private string UpIcon = "bi bi-chevron-compact-up";
        private string DownIcon = "bi bi-chevron-compact-down";
        public SortOrder SortedOrder { get; set; }
        public string SortedProperty { get; set; } = null!;
        public string SortedExpression { get; private set; } = null!;

        private List<SortableColumn> sortableColumns = new List<SortableColumn>();

        public void AddColumn(string colname, bool isDefaultColumn = false)
        {
            SortableColumn column = sortableColumns.Where(x => x.ColumnName.ToLower() == colname.ToLower()).SingleOrDefault()!;
            if (column == null)
            {
                sortableColumns.Add(new SortableColumn { ColumnName = colname });
            }

            if (isDefaultColumn == true || sortableColumns.Count == 1)
            {
                SortedProperty = colname;
                SortedOrder = SortOrder.Ascending;
            }

        }

        public SortableColumn GetColumn(string colname)
        {
            SortableColumn column = sortableColumns.Where(x => x.ColumnName.ToLower() == colname.ToLower()).SingleOrDefault()!;
            if (column == null)
            {
                sortableColumns.Add(new SortableColumn { ColumnName = colname });
            }
            return column;
        }

        public void ApplySort(string sortExpression)
        {
            if (sortExpression == null)
                sortExpression = "";

            if (sortExpression == "")
                sortExpression = SortedProperty;

            sortExpression = sortExpression.ToLower();
            SortedExpression = sortExpression;

            foreach (SortableColumn column in sortableColumns)
            {
                column.SortIcon = "";
                column.SortExpression = column.ColumnName;

                if (sortExpression == column.ColumnName.ToLower())
                {
                    SortedOrder = SortOrder.Ascending;
                    SortedProperty = column.ColumnName;
                    column.SortExpression = column.ColumnName + "_desc";
                    column.SortIcon = DownIcon;
                }
                else if (sortExpression == column.ColumnName.ToLower() + "_desc")
                {
                    SortedOrder = SortOrder.Descending;
                    SortedProperty = column.ColumnName;
                    column.SortExpression = column.ColumnName;
                    column.SortIcon = UpIcon;
                }
            }

        }
    }


    public class SortableColumn
    {
        public string ColumnName { get; set; } = null!;
        public string SortExpression { get; set; } = null!;
        public string SortIcon { get; set; } = null!;
    }
}
