using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public partial class AddTaskViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Now;

        [ObservableProperty]
        private string _priority = "Medium";

        [ObservableProperty]
        private string _category;

        [ObservableProperty]
        private string _recurrenceType = "None"; // Default to "None"

        [ObservableProperty]
        private int _recurrenceInterval = 1;


        private readonly DatabaseContext _databaseContext;
        private TaskModel _taskToEdit;

        // Constructor for adding a new task
        public AddTaskViewModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // Constructor for editing an existing task
        public AddTaskViewModel(DatabaseContext databaseContext, TaskModel taskToEdit)
        {
            _databaseContext = databaseContext;
            _taskToEdit = taskToEdit;

            // Populate fields with existing data
            Title = taskToEdit.Title;
            Description = taskToEdit.Description;
            DueDate = taskToEdit.DueDate;
            Priority = taskToEdit.Priority;
            Category = taskToEdit.Category;
            //Simplified
            if (taskToEdit.Recurrence != null)
            {
                RecurrenceType = taskToEdit.Recurrence.Type;
                RecurrenceInterval = taskToEdit.Recurrence.Interval;
            }
            else
            {
                RecurrenceType = "None"; //If no Recurrence.
            }
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
                _taskToEdit.Title = Title;
                _taskToEdit.Description = Description;
                _taskToEdit.DueDate = DueDate;
                _taskToEdit.Priority = Priority;
                _taskToEdit.Category = Category;
                _taskToEdit.Recurrence = recurrence; // Update recurrence
                await _databaseContext.UpdateTaskAsync(_taskToEdit);
            }
            else
            {
                // Create a new task
                var newTask = new TaskModel
                {
                    Title = Title,
                    Description = Description,
                    DueDate = DueDate,
                    Priority = Priority,
                    Category = Category,
                    Recurrence = recurrence, // Set recurrence
                };
                await _databaseContext.AddTaskAsync(newTask);
            }

            await Shell.Current.GoToAsync(".."); // Navigate back
        }
    }
}