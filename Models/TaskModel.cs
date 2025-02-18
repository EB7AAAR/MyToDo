using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.Generic;
using System.Text.Json;
using System.ComponentModel; // Manually implement INotifyPropertyChanged

namespace MyToDo.Models
{
    public partial class TaskModel : BaseEntity, INotifyPropertyChanged
    {
        [ObservableProperty]
        private bool _isDragging;
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        //public bool IsRecurring { get; set; } Remove the IsRecurring

        private RecurrencePattern _recurrence;

        [Ignore]
        public RecurrencePattern Recurrence
        {
            get { return _recurrence; }
            set
            {
                if (_recurrence != value)
                {
                    _recurrence = value;
                    RecurrenceJson = JsonSerializer.Serialize(_recurrence); // Serialize on set
                    OnPropertyChanged(nameof(Recurrence)); // Explicitly notify
                }
            }
        }


        private string _recurrenceJson;

        public string RecurrenceJson
        {
            get { return _recurrenceJson; }
            set
            {
                if (_recurrenceJson != value)
                {
                    _recurrenceJson = value;
                    if (!string.IsNullOrEmpty(_recurrenceJson))
                    {
                        try
                        {
                            Recurrence = JsonSerializer.Deserialize<RecurrencePattern>(_recurrenceJson);
                        }
                        catch (JsonException)
                        {
                            Recurrence = new RecurrencePattern { Type = "Daily", Interval = 1 };
                        }
                    }
                    OnPropertyChanged(nameof(RecurrenceJson)); // Explicitly notify

                }
            }
        }


        private List<Subtask> _subtasks = new();

        [Ignore]
        public List<Subtask> Subtasks
        {
            get { return _subtasks; }
            set
            {
                if (_subtasks != value)
                {
                    _subtasks = value;
                    SubtasksJson = JsonSerializer.Serialize(_subtasks); // Serialize on set
                    OnPropertyChanged(nameof(Subtasks)); // Explicitly notify

                }
            }
        }

        private string _subtasksJson;

        public string SubtasksJson
        {
            get { return _subtasksJson; }
            set
            {
                if (_subtasksJson != value)
                {
                    _subtasksJson = value;

                    if (!string.IsNullOrEmpty(_subtasksJson))
                    {
                        try
                        {
                            Subtasks = JsonSerializer.Deserialize<List<Subtask>>(_subtasksJson);
                        }
                        catch (JsonException)
                        {
                            Subtasks = new List<Subtask>();
                        }
                    }
                    OnPropertyChanged(nameof(SubtasksJson)); // Explicitly notify
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Required by INotifyPropertyChanged
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}