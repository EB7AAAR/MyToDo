using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace MyToDo.Models
{
    public partial class TaskModel : BaseEntity
    {
        [ObservableProperty]
        private bool _isDragging;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private DateTime _dueDate;

        [ObservableProperty]
        private string _priority;

        [ObservableProperty]
        private string _category;

        public string FirebaseKey { get; set; }

        private RecurrencePattern _recurrence;

        [Ignore]
        public RecurrencePattern Recurrence
        {
            get => _recurrence;
            set
            {
                if (SetProperty(ref _recurrence, value))
                {
                    RecurrenceJson = JsonSerializer.Serialize(_recurrence);
                }
            }
        }

        [ObservableProperty] // This generates a RecurrenceJson property.
        private string _recurrenceJson;


        private List<Subtask> _subtasks = new();

        [Ignore]
        public List<Subtask> Subtasks
        {
            get => _subtasks;
            set
            {
                if (SetProperty(ref _subtasks, value))
                {
                    SubtasksJson = JsonSerializer.Serialize(_subtasks);
                }
            }
        }

        [ObservableProperty]
        private string _subtasksJson;
    }
}