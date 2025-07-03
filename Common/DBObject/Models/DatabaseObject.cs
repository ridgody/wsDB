namespace wsDB.Common.DBObject.Models
{
    public class DatabaseObject
    {
        public string Owner { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        
        public string DisplayText => $"{Owner}.{ObjectName} ({ObjectType})";
    }
}