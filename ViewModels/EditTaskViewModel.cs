// EditTaskViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Diagnostics;


namespace MyToDo.ViewModels;

public partial class EditTaskViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private TaskModel _task;

    [ObservableProperty] private ObservableCollection<Subtask> _subtasks = new();


    private readonly DatabaseContext _databaseContext;

    public EditTaskViewModel(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;

    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("taskId") && query["taskId"] is string taskIdString && int.TryParse(taskIdString, out int taskId))
        {
            await LoadTask(taskId);

        }
    }

    private async Task LoadTask(int taskId)
    {
        Task = await _databaseContext.GetTaskByIdAsync(taskId);

        if (Task == null)
        {
            // Handle case where task is not found (e.g., show error, navigate back)
            Debug.WriteLine($"---> Task with ID {taskId} not found.");
            await Shell.Current.GoToAsync(".."); // Or show an error message
            return;
        }

        //get subtasks from json and assign it to observalblecollection
        if (!string.IsNullOrEmpty(Task.SubtasksJson))
        {
            try
            {
                Subtasks = new ObservableCollection<Subtask>(JsonSerializer.Deserialize<List<Subtask>>(Task.SubtasksJson));
            }
            catch (JsonException ex)
            {
                Subtasks = new ObservableCollection<Subtask>();
                Debug.WriteLine($"---> Error deserializing subtasks: {ex.Message}");
            }
        }
        //load the receurrence
        if (!string.IsNullOrEmpty(Task.RecurrenceJson))
        {
            try
            {
                Task.Recurrence = JsonSerializer.Deserialize<RecurrencePattern>(Task.RecurrenceJson);
                OnPropertyChanged(nameof(Task)); // VERY IMPORTANT: Notify the UI
            }
            catch (JsonException ex)
            {
                Task.Recurrence = null; // Or handle as appropriate
                Debug.WriteLine($"---> Error deserializing RecurrenceJson: {ex.Message}");
            }

        }
        //if there is no recurence set the values by default
        else
        {
            Task.Recurrence = new() { Type = "None", Interval = 1 };
            OnPropertyChanged(nameof(Task)); // VERY IMPORTANT: Notify the UI

        }
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        if (Task == null) return;
        // Update the task
        Task.SubtasksJson = JsonSerializer.Serialize(Subtasks);
        //save recurrece
        Task.RecurrenceJson = JsonSerializer.Serialize(Task.Recurrence);
        await _databaseContext.UpdateTaskAsync(Task);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void AddSubtask()
    {
        Subtasks.Add(new Subtask { Description = "", IsCompleted = false });
    }

    [RelayCommand]
    private void RemoveSubtask(Subtask subtask)
    {
        if (subtask == null) return;
        Subtasks.Remove(subtask);
    }
}