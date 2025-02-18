using MyToDo.ViewModels;

namespace MyToDo.Views;

public partial class AddTaskView : ContentPage
{
    public AddTaskView(AddTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}