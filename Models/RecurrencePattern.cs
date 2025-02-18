namespace MyToDo.Models
{
    public class RecurrencePattern
    {
        public string Type { get; set; } // "Daily", "Weekly", "Monthly"
        public int Interval { get; set; } // e.g., every 2 weeks
    }
}
