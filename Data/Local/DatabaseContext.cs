// DatabaseContext.cs
using MyToDo.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

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
            try
            {
                return await _database.Table<TaskModel>().ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"---> Error in GetAllTasksAsync: {ex}");
                throw;
            }
        }
        //add this function
        public async Task<TaskModel> GetTaskByIdAsync(int taskId)
        {
            return await _database.Table<TaskModel>().FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<int> AddTaskAsync(TaskModel task)
        {
            try
            {
                return await _database.InsertAsync(task);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"---> Error in AddTaskAsync: {ex}");
                throw;
            }
        }

        public async Task<int> UpdateTaskAsync(TaskModel task)
        {
            try
            {
                return await _database.UpdateAsync(task);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"---> Error in UpdateTaskAsync: {ex}");
                throw;
            }
        }

        public async Task<int> DeleteTaskAsync(TaskModel task)
        {
            try
            {
                return await _database.DeleteAsync(task);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"---> Error in DeleteTaskAsync: {ex}");
                throw;
            }
        }
    }
}