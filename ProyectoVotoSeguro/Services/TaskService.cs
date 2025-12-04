using ProyectoVotoSeguro.DTOs;
using ProyectoVotoSeguro.Models;

namespace ProyectoVotoSeguro.Services
{
    public class TaskService : ITaskService
    {
        private readonly FirebaseServices _firebaseService;
        private readonly IAuthService _authService;

        public TaskService(FirebaseServices firebaseService, IAuthService authService)
        {
            _firebaseService = firebaseService;
            _authService = authService;
        }

        public async Task<TaskItem> CreateTask(CreateTaskDto createTaskDto, string createdByUserId, string createdByUserName)
        {
            try
            {
                // 1. Verificar que el usuario asignado existe
                var assignedUser = await _authService.GetUserById(createTaskDto.AssignedToUserId);
                if (assignedUser == null)
                {
                    throw new Exception("El usuario asignado no existe");
                }

                // 2. Crear la tarea
                var taskId = Guid.NewGuid().ToString();
                var task = new TaskItem
                {
                    Id = taskId,
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    AssignedToUserId = createTaskDto.AssignedToUserId,
                    AssignedToUserName = assignedUser.Fullname,
                    CreatedByUserId = createdByUserId,
                    CreatedByUserName = createdByUserName,
                    DueDate = createTaskDto.DueDate,
                    Priority = createTaskDto.Priority,
                    Status = "pending",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                // 3. Guardar en Firestore
                var tasksCollection = _firebaseService.GetCollection("tasks");
                await tasksCollection.Document(taskId).SetAsync(task);

                return task;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear tarea: {ex.Message}");
            }
        }

        public async Task<TaskItem?> GetTaskById(string taskId)
        {
            try
            {
                var taskDoc = await _firebaseService
                    .GetCollection("tasks")
                    .Document(taskId)
                    .GetSnapshotAsync();

                if (!taskDoc.Exists)
                {
                    return null;
                }

                return taskDoc.ConvertTo<TaskItem>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<TaskItem>> GetAllTasks()
        {
            try
            {
                var tasksCollection = _firebaseService.GetCollection("tasks");
                var snapshot = await tasksCollection.GetSnapshotAsync();

                var tasks = new List<TaskItem>();
                foreach (var document in snapshot.Documents)
                {
                    tasks.Add(document.ConvertTo<TaskItem>());
                }

                return tasks.OrderByDescending(t => t.CreatedAt).ToList();
            }
            catch (Exception)
            {
                return new List<TaskItem>();
            }
        }

        public async Task<List<TaskItem>> GetTasksByUserId(string userId)
        {
            try
            {
                var tasksCollection = _firebaseService.GetCollection("tasks");
                var query = tasksCollection.WhereEqualTo("AssignedToUserId", userId);
                var snapshot = await query.GetSnapshotAsync();

                var tasks = new List<TaskItem>();
                foreach (var document in snapshot.Documents)
                {
                    tasks.Add(document.ConvertTo<TaskItem>());
                }

                return tasks.OrderByDescending(t => t.CreatedAt).ToList();
            }
            catch (Exception)
            {
                return new List<TaskItem>();
            }
        }

        public async Task<TaskItem?> UpdateTask(string taskId, UpdateTaskDto updateTaskDto)
        {
            try
            {
                var taskDoc = _firebaseService.GetCollection("tasks").Document(taskId);
                var snapshot = await taskDoc.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return null;
                }

                var updates = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(updateTaskDto.Title))
                {
                    updates["Title"] = updateTaskDto.Title;
                }

                if (!string.IsNullOrEmpty(updateTaskDto.Description))
                {
                    updates["Description"] = updateTaskDto.Description;
                }

                if (!string.IsNullOrEmpty(updateTaskDto.Priority))
                {
                    updates["Priority"] = updateTaskDto.Priority;
                }

                if (!string.IsNullOrEmpty(updateTaskDto.Status))
                {
                    updates["Status"] = updateTaskDto.Status;
                }

                if (updateTaskDto.DueDate.HasValue)
                {
                    updates["DueDate"] = updateTaskDto.DueDate.Value;
                }

                if (!string.IsNullOrEmpty(updateTaskDto.AssignedToUserId))
                {
                    var assignedUser = await _authService.GetUserById(updateTaskDto.AssignedToUserId);
                    if (assignedUser != null)
                    {
                        updates["AssignedToUserId"] = updateTaskDto.AssignedToUserId;
                        updates["AssignedToUserName"] = assignedUser.Fullname;
                    }
                }

                if (updates.Count > 0)
                {
                    await taskDoc.UpdateAsync(updates);
                }

                var updatedSnapshot = await taskDoc.GetSnapshotAsync();
                return updatedSnapshot.ConvertTo<TaskItem>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar tarea: {ex.Message}");
            }
        }

        public async Task<bool> DeleteTask(string taskId)
        {
            try
            {
                var taskDoc = _firebaseService.GetCollection("tasks").Document(taskId);
                var snapshot = await taskDoc.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return false;
                }

                await taskDoc.DeleteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TaskItem?> CompleteTask(string taskId, string userId)
        {
            try
            {
                var taskDoc = _firebaseService.GetCollection("tasks").Document(taskId);
                var snapshot = await taskDoc.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return null;
                }

                var task = snapshot.ConvertTo<TaskItem>();

                // Verificar que el usuario sea el asignado
                if (task.AssignedToUserId != userId)
                {
                    throw new Exception("Solo el usuario asignado puede completar esta tarea");
                }

                // Marcar como completada
                var updates = new Dictionary<string, object>
                {
                    { "IsCompleted", true },
                    { "CompletedAt", DateTime.UtcNow },
                    { "Status", "completed" }
                };

                await taskDoc.UpdateAsync(updates);

                var updatedSnapshot = await taskDoc.GetSnapshotAsync();
                return updatedSnapshot.ConvertTo<TaskItem>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al completar tarea: {ex.Message}");
            }
        }
    }
}