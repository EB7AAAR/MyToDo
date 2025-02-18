using MyToDo.Views;

namespace MyToDo;

public partial class App : Application
{
    public App(TaskListView taskListView) // Correct parameter type
    {
        InitializeComponent();

        MainPage = new AppShell();
                                   
                                   
    }
}
