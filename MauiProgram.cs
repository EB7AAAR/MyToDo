using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MyToDo.Data.Local;
using MyToDo.Data.Remote;
using MyToDo.ViewModels;
using MyToDo.Views;

namespace MyToDo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<FirebaseService>(); // Add this line
        // Register DatabaseContext
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tasks.db3");
        builder.Services.AddSingleton(new DatabaseContext(dbPath));

        // Register ViewModels and Views
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<TaskListView>();
        builder.Services.AddTransient<AddTaskViewModel>();
        builder.Services.AddTransient<AddTaskView>();
        //Register EditTask
        builder.Services.AddTransient<EditTaskViewModel>();
        builder.Services.AddTransient<EditTaskView>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
