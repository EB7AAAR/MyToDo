using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using MyToDo.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TaskModel> _tasks = new();

        private readonly DatabaseContext _databaseContext;
        private bool _isLoaded = false; // Flag to prevent initial double loading

        public HomeViewModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // Call this method to refresh the task list.
        public async Task LoadTasksAsync()
        {
            if (_isLoaded) return; // Prevent double loading ON STARTUP

            Debug.WriteLine("---> LoadTasksAsync called");

            _isLoaded = true; // Set the flag - but ONLY on initial load
            Tasks.Clear();
            var loadedTasks = await _databaseContext.GetAllTasksAsync();

            Debug.WriteLine($"---> Loaded {loadedTasks.Count} tasks from database");

            foreach (var task in loadedTasks)
            {
                Debug.WriteLine($"---> Task: {task.Description}, Due: {task.DueDate}"); // Show Description
                Tasks.Add(task);
            }
        }

        // IMPORTANT: Add this method to reset the flag
        public void ResetIsLoaded()
        {
            _isLoaded = false;
        }

        [RelayCommand]
        private async Task AddTask()
        {
            var addTaskViewModel = new AddTaskViewModel(_databaseContext);
            await Shell.Current.GoToAsync(nameof(AddTaskView), new Dictionary<string, object>
            {
                {nameof(AddTaskViewModel), addTaskViewModel }
            });
        }

        [RelayCommand]
        private async Task GoToEditTask(TaskModel task)
        {
            if (task == null) return;

            var addTaskViewModel = new AddTaskViewModel(_databaseContext, task);
            await Shell.Current.GoToAsync($"{nameof(AddTaskView)}?id={task.Id}",
               new Dictionary<string, object>
               {
                    {nameof(AddTaskViewModel), addTaskViewModel }
               });
        }

        [RelayCommand]
        private async Task DeleteTask(TaskModel task)
        {
            if (task == null) return;

            await _databaseContext.DeleteTaskAsync(task);
            Tasks.Remove(task); // Remove from the observable collection too
        }
    }
}