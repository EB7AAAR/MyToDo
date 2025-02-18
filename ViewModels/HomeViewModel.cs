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

        public HomeViewModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            // Data loading is handled in TaskListView.OnAppearing
        }

        public async Task LoadTasksAsync()
        {
            Debug.WriteLine("---> LoadTasksAsync called");

            Tasks.Clear();
            var loadedTasks = await _databaseContext.GetAllTasksAsync();

            Debug.WriteLine($"---> Loaded {loadedTasks.Count} tasks from database");

            foreach (var task in loadedTasks)
            {
                Debug.WriteLine($"---> Task: {task.Title}, Due: {task.DueDate}");
                Tasks.Add(task);
            }
        }

        [RelayCommand]
        private async Task AddTask()
        {
            // Create a new AddTaskViewModel with just the database context
            var addTaskViewModel = new AddTaskViewModel(_databaseContext);
            await Shell.Current.GoToAsync(nameof(AddTaskView), new Dictionary<string, object>
            {
                {nameof(AddTaskViewModel), addTaskViewModel }
            });
        }

        [RelayCommand] // Add this for Edit functionality
        private async Task GoToEditTask(TaskModel task)
        {
            if (task == null) return;

            // Create a new AddTaskViewModel, passing BOTH the database context AND the task to edit
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