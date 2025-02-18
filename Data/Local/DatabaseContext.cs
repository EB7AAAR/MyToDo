using MyToDo.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyToDo.Data.Local
{
    public class DatabaseContext
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseContext(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<TaskModel>().Wait();
        }

        public async Task<List<TaskModel>> GetAllTasksAsync()
        {
            return await _database.Table<TaskModel>().ToListAsync();
        }

        public async Task<int> AddTaskAsync(TaskModel task)
        {
            return await _database.InsertAsync(task);
        }

        public async Task<int> UpdateTaskAsync(TaskModel task)
        {
            return await _database.UpdateAsync(task);
        }

        public async Task<int> DeleteTaskAsync(TaskModel task)
        {
            return await _database.DeleteAsync(task);
        }
    }
}