// TaskListView.xaml.cs
using MyToDo.ViewModels;

namespace MyToDo.Views;

public partial class TaskListView : ContentPage
{
    public TaskListView(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeViewModel vm)
        {
            vm.ResetIsLoaded(); // Reset the flag BEFORE loading
            await vm.LoadTasksAsync(); // Reload data
        }
    }
}