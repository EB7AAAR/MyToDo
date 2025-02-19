using MyToDo.Views;

namespace MyToDo;

public partial class App : Application
{
    public App(TaskListView taskListView)
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}