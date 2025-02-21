using MyToDo.Views;

namespace MyToDo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(AddTaskView), typeof(AddTaskView));
        Routing.RegisterRoute(nameof(EditTaskView), typeof(EditTaskView)); // Add this line

    }
}