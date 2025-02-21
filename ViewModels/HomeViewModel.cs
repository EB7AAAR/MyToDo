// ViewModels/HomeViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using MyToDo.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using System.Windows.Input;
using MyToDo.Data.Remote;

namespace MyToDo.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TaskModel> _tasks = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage;

        // Drag and drop commands
        public ICommand DragStartingCommand { get; }
        public ICommand DropOverCommand { get; }
        public ICommand DropCommand { get; }

        private readonly DatabaseContext _databaseContext;
        private readonly FirebaseService _firebaseService;
        private bool _isLoaded = false;

        public HomeViewModel(DatabaseContext databaseContext, FirebaseService firebaseService)
        {
            _databaseContext = databaseContext;
            _firebaseService = firebaseService;
            // Initialize the commands
            DragStartingCommand = new Command<TaskModel>(OnDragStarting);
            DropOverCommand = new Command<TaskModel>(OnDropOver);
            DropCommand = new Command<TaskModel>(OnDrop);
        }

        // ... (LoadTasksAsync, ResetIsLoaded, AddTask, GoToEditTask, DeleteTask - as before) ...
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

            // Create the AddTaskViewModel with the task to edit.
            var addTaskViewModel = new AddTaskViewModel(_databaseContext, task);

            // Pass the ViewModel in the navigation parameters.
            await Shell.Current.GoToAsync(nameof(AddTaskView), new Dictionary<string, object>
            {
                { nameof(AddTaskViewModel), addTaskViewModel }
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
        private TaskModel _draggedTask; // Store the task being dragged

        private void OnDragStarting(TaskModel task)
        {
            _draggedTask = task;
        }

        private void OnDropOver(TaskModel task)
        {
            // Could add visual feedback here (e.g., highlighting the drop target)
        }

        private void OnDrop(TaskModel dropTask)
        {
            if (_draggedTask == null || dropTask == null)
            {
                return;
            }

            if (ReferenceEquals(_draggedTask, dropTask))
            {
                return; // Dropped on itself
            }

            int dragIndex = Tasks.IndexOf(_draggedTask);
            int dropIndex = Tasks.IndexOf(dropTask);

            if (dragIndex == -1 || dropIndex == -1)
            {
                return;
            }

            Tasks.RemoveAt(dragIndex);
            Tasks.Insert(dropIndex, _draggedTask);

            _draggedTask = null;
        }
    }
}