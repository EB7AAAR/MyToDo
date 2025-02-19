using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics; // Make sure to include this for Debug.WriteLine

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
        private TaskModel _taskToEdit;

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
            _taskToEdit = taskToEdit;

            Description = taskToEdit.Description;
            DueDate = taskToEdit.DueDate;
            Priority = taskToEdit.Priority;
            Category = taskToEdit.Category;

            if (taskToEdit.Recurrence != null)
            {
                RecurrenceType = taskToEdit.Recurrence.Type;
                RecurrenceInterval = taskToEdit.Recurrence.Interval;
            }
            else
            {
                RecurrenceType = "None";
            }

            // IMPORTANT: Load subtasks correctly
            if (taskToEdit.Subtasks != null)
            {
                foreach (var sub in taskToEdit.Subtasks)
                {
                    Subtasks.Add(new Subtask { Description = sub.Description, IsCompleted = sub.IsCompleted }); // Create new instances
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
                _taskToEdit.Recurrence = recurrence; // Correctly assign recurrence
                _taskToEdit.Subtasks = new List<Subtask>(Subtasks);  //CRUCIAL: Create a *new* list.

                await _databaseContext.UpdateTaskAsync(_taskToEdit);
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
                    Recurrence = recurrence, // Correctly assign recurrence
                    Subtasks = new List<Subtask>(Subtasks) // CRUCIAL: Create a *new* list.
                };
                await _databaseContext.AddTaskAsync(newTask);
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}