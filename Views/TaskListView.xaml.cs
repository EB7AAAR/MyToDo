using MyToDo.ViewModels;

namespace MyToDo.Views;

public partial class TaskListView : ContentPage
{
	public TaskListView(HomeViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

        // Call LoadTasksAsync *after* setting the BindingContext:
        Loaded += async (sender, e) =>
        {
            if (BindingContext is HomeViewModel vm)
            {
                await vm.LoadTasksAsync();
            }
        };


    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeViewModel vm)
        {
            await vm.LoadTasksAsync(); // Reload data!
        }
    }
}