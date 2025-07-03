namespace wsDB.OralceExecutionPlan.DbmsXplan.Models
{  

    // 데이터 클래스들
    public class PropertyInfo
    {
        public string Property { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class StatisticInfo
    {
        public string StatName { get; set; } = "";
        public string StatValue { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class ColumnInfo
    {
        public string ColumnName { get; set; } = "";
        public string DataType { get; set; } = "";
        public string DataLength { get; set; } = "";
        public string Nullable { get; set; } = "";
        public string DefaultValue { get; set; } = "";
    }


    public class IndexColumnInfo
    {
        public string ColumnName { get; set; } = "";
        public string ColumnPosition { get; set; } = "";
        public string DescendFlag { get; set; } = "";
    }
  }