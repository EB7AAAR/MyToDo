// AddTaskView.xaml.cs
using MyToDo.ViewModels;
using System.Threading.Tasks; // Make sure this is included

namespace MyToDo.Views;

public partial class AddTaskView : ContentPage
{
    public AddTaskView(AddTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing() // Make this async void
    {
        base.OnAppearing();

        if (this.FindByName("DescriptionEditor") is Editor descriptionEditor)
        {
            // Add a short delay before focusing.  This is the key!
            await Task.Delay(100); // Delay for 100 milliseconds
            descriptionEditor.Focus();
        }
    }
}