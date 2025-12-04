using ProyectoVotoSeguro.DTOs;
using ProyectoVotoSeguro.Models;

namespace ProyectoVotoSeguro.Services
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTask(CreateTaskDto createTaskDto, string createdByUserId, string createdByUserName);
        Task<TaskItem?> GetTaskById(string taskId);
        Task<List<TaskItem>> GetAllTasks();
        Task<List<TaskItem>> GetTasksByUserId(string userId);
        Task<TaskItem?> UpdateTask(string taskId, UpdateTaskDto updateTaskDto);
        Task<bool> DeleteTask(string taskId);
        Task<TaskItem?> CompleteTask(string taskId, string userId);
    }
}