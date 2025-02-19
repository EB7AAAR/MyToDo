using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using MyToDo.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace MyToDo.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TaskModel> _tasks = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _errorMessage;


        private readonly DatabaseContext _databaseContext;
        private bool _isLoaded = false;

        public HomeViewModel(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task LoadTasksAsync()
        {
            if (_isLoaded) return;

            IsBusy = true;
            _isLoaded = true;
            ErrorMessage = null;

            try
            {
                Tasks.Clear();
                var loadedTasks = await _databaseContext.GetAllTasksAsync();

                foreach (var task in loadedTasks)
                {
                    if (!string.IsNullOrEmpty(task.SubtasksJson))
                    {
                        try
                        {
                            task.Subtasks = JsonSerializer.Deserialize<List<Subtask>>(task.SubtasksJson);
                        }
                        catch (JsonException)
                        {
                            task.Subtasks = new List<Subtask>();
                            Debug.WriteLine($"---> Error deserializing subtasks for task ID {task.Id}");
                        }
                    }
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load tasks: {ex.Message}";
                Debug.WriteLine($"---> Error loading tasks: {ex}");
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


        public void ResetIsLoaded()
        {
            _isLoaded = false;
        }

        [RelayCommand]
        private async Task AddTask()
        {
            ErrorMessage = null;
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
            ErrorMessage = null;
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
            ErrorMessage = null;
            try
            {
                await _databaseContext.DeleteTaskAsync(task);
                Tasks.Remove(task);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to Delete tasks: {ex.Message}";
                Debug.WriteLine($"---> Error Deleting tasks: {ex}");
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }

        }

        [RelayCommand]
        private async Task RefreshTasks()
        {
            IsRefreshing = true;
            _isLoaded = false;
            ErrorMessage = null;
            try
            {
                await LoadTasksAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}