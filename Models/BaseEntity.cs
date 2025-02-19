// Models/BaseEntity.cs
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;

namespace MyToDo.Models
{
    public abstract class BaseEntity : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}