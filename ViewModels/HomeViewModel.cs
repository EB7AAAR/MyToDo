// ViewModels/HomeViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Data.Remote;
using MyToDo.Models;
using MyToDo.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace MyToDo.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<TaskModel> _tasks = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage;

    private readonly DatabaseContext _databaseContext;
    private readonly FirebaseService _firebaseService;  // Keep this if you plan to use Firebase
    private bool _isLoaded = false;

    public HomeViewModel(DatabaseContext databaseContext, FirebaseService firebaseService)
    {
        _databaseContext = databaseContext;
        _firebaseService = firebaseService; // Keep this if you plan to use Firebase
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
        await Shell.Current.GoToAsync(nameof(AddTaskView)); // No need to pass the ViewModel
    }

    [RelayCommand]
    private async Task GoToEditTask(TaskModel task)
    {
        if (task == null) return;
        ErrorMessage = null;

        // Use query parameters to pass the task ID
        await Shell.Current.GoToAsync($"{nameof(EditTaskView)}?taskId={task.Id}");
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

    private TaskModel _draggedTask;

    [RelayCommand]
    private void DragStarting(TaskModel task)
    {
        _draggedTask = task;
    }

    [RelayCommand]
    private void DropOver(TaskModel task)
    {
        // Could add visual feedback here (e.g., highlighting the drop target)
    }

    [RelayCommand]
    private void Drop(TaskModel dropTask)
    {
        if (_draggedTask == null || dropTask == null || ReferenceEquals(_draggedTask, dropTask))
        {
            return;
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