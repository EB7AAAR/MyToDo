// EditTaskView.xaml.cs
using MyToDo.ViewModels;

namespace MyToDo.Views;

public partial class EditTaskView : ContentPage
{
    public EditTaskView(EditTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}