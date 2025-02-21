// AddTaskViewModel.cs (Full code, with explanations)

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using System.Collections.ObjectModel;
using System.Text.Json; // Ensure this is present

namespace MyToDo.ViewModels
{
    public partial class AddTaskViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Now;

        [ObservableProperty]
        private string _priority = "Medium";

        [ObservableProperty]
        private string _category;

        [ObservableProperty]
        private string _recurrenceType = "None";

        [ObservableProperty]
        private int _recurrenceInterval = 1;

        [ObservableProperty]
        private ObservableCollection<Subtask> _subtasks = new();

        private readonly DatabaseContext _databaseContext;
        private TaskModel _taskToEdit; // This holds the task being edited

        // Constructor for adding a new task
        public AddTaskViewModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            AddSubtask(); // Start with one empty subtask
            Category = "Work"; // Default category
        }

        // Constructor for editing an existing task
        public AddTaskViewModel(DatabaseContext databaseContext, TaskModel taskToEdit)
        {
            _databaseContext = databaseContext;
            _taskToEdit = taskToEdit; // Store the task to be edited

            // Populate the properties with the existing task's data
            Description = taskToEdit.Description;
            DueDate = taskToEdit.DueDate;
            Priority = taskToEdit.Priority;
            Category = taskToEdit.Category;

            // Handle Recurrence (check for null)
            if (taskToEdit.Recurrence != null)
            {
                RecurrenceType = taskToEdit.Recurrence.Type;
                RecurrenceInterval = taskToEdit.Recurrence.Interval;
            }
            else
            {
                RecurrenceType = "None"; // Default if no recurrence
            }
            // Deserialize Subtasks from JSON, handle potential errors.
            if (!string.IsNullOrEmpty(taskToEdit.SubtasksJson))
            {
                try
                {
                    Subtasks = new ObservableCollection<Subtask>(JsonSerializer.Deserialize<List<Subtask>>(taskToEdit.SubtasksJson));
                }
                catch (JsonException)
                {
                    // Handle JSON deserialization errors (e.g., log, show error)
                    Subtasks = new ObservableCollection<Subtask>(); // Reset to empty
                }
            }


        }

        [RelayCommand]
        private void AddSubtask()
        {
            Subtasks.Add(new Subtask { Description = "", IsCompleted = false });
        }

        [RelayCommand]
        private void RemoveSubtask(Subtask subtask)
        {
            Subtasks.Remove(subtask);
        }

        [RelayCommand]
        private async Task SaveTask()
        {
            // Create the RecurrencePattern object (if needed)
            RecurrencePattern recurrence = null;
            if (RecurrenceType != "None")
            {
                recurrence = new RecurrencePattern { Type = RecurrenceType, Interval = RecurrenceInterval };
            }

            if (_taskToEdit != null)
            {
                // Update existing task
                _taskToEdit.Description = Description;
                _taskToEdit.DueDate = DueDate;
                _taskToEdit.Priority = Priority;
                _taskToEdit.Category = Category;
                _taskToEdit.Recurrence = recurrence; // Assign the RecurrencePattern
                _taskToEdit.SubtasksJson = JsonSerializer.Serialize(Subtasks); // Serialize Subtasks

                await _databaseContext.UpdateTaskAsync(_taskToEdit); // Update in the database
            }
            else
            {
                // Create a new task
                var newTask = new TaskModel
                {
                    Description = Description,
                    DueDate = DueDate,
                    Priority = Priority,
                    Category = Category,
                    Recurrence = recurrence, // Assign the RecurrencePattern
                    SubtasksJson = JsonSerializer.Serialize(Subtasks) // Serialize Subtasks

                };
                await _databaseContext.AddTaskAsync(newTask); // Add to the database
            }

            await Shell.Current.GoToAsync(".."); // Navigate back
        }
    }
}