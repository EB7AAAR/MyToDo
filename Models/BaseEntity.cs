using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace MyToDo.Models
{
    public abstract class BaseEntity : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }  // Unique identifier for SQLite
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}
