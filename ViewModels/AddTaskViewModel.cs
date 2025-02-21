// AddTaskViewModel.cs (Simplified for *adding* only)
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDo.Data.Local;
using MyToDo.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyToDo.ViewModels;

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

    public AddTaskViewModel(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        AddSubtask(); // Start with one empty subtask
        Category = "Work";
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

        // Create a new task (no _taskToEdit logic here)
        var newTask = new TaskModel
        {
            Description = Description,
            DueDate = DueDate,
            Priority = Priority,
            Category = Category,
            Recurrence = recurrence,
            SubtasksJson = JsonSerializer.Serialize(Subtasks)
        };

        await _databaseContext.AddTaskAsync(newTask);
        await Shell.Current.GoToAsync("..");
    }
}