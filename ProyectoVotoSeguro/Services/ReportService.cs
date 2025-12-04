using ProyectoVotoSeguro.Models;

namespace ProyectoVotoSeguro.Services
{
    public class ReportService : IReportService
    {
        private readonly FirebaseServices _firebaseService;
        private readonly ITaskService _taskService;

        public ReportService(FirebaseServices firebaseService, ITaskService taskService)
        {
            _firebaseService = firebaseService;
            _taskService = taskService;
        }

        public async Task<TaskStatistics> GetTaskStatistics()
        {
            try
            {
                var allTasks = await _taskService.GetAllTasks();

                var statistics = new TaskStatistics
                {
                    TotalTasks = allTasks.Count,
                    CompletedTasks = allTasks.Count(t => t.IsCompleted),
                    PendingTasks = allTasks.Count(t => t.Status == "pending"),
                    InProgressTasks = allTasks.Count(t => t.Status == "in-progress"),
                    HighPriorityTasks = allTasks.Count(t => t.Priority == "high"),
                    MediumPriorityTasks = allTasks.Count(t => t.Priority == "medium"),
                    LowPriorityTasks = allTasks.Count(t => t.Priority == "low"),
                    OverdueTasks = allTasks.Count(t => t.DueDate.HasValue && 
                                                       t.DueDate.Value < DateTime.UtcNow && 
                                                       !t.IsCompleted)
                };

                // Calcular tasa de completitud
                if (statistics.TotalTasks > 0)
                {
                    statistics.CompletionRate = Math.Round(
                        (double)statistics.CompletedTasks / statistics.TotalTasks * 100, 2
                    );
                }

                // Tareas por usuario
                var tasksByUser = allTasks
                    .GroupBy(t => new { t.AssignedToUserId, t.AssignedToUserName })
                    .Select(g => new TasksByUser
                    {
                        UserId = g.Key.AssignedToUserId,
                        UserName = g.Key.AssignedToUserName,
                        TotalTasks = g.Count(),
                        CompletedTasks = g.Count(t => t.IsCompleted),
                        PendingTasks = g.Count(t => !t.IsCompleted)
                    })
                    .OrderByDescending(u => u.TotalTasks)
                    .ToList();

                statistics.TasksByUsers = tasksByUser;

                // Tareas por prioridad
                var tasksByPriority = allTasks
                    .GroupBy(t => t.Priority)
                    .Select(g => new TasksByPriority
                    {
                        Priority = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                statistics.TasksByPriorities = tasksByPriority;

                // Tareas por estado
                var tasksByStatus = allTasks
                    .GroupBy(t => t.Status)
                    .Select(g => new TasksByStatus
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                statistics.TasksByStatuses = tasksByStatus;

                return statistics;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}");
            }
        }

        public async Task<TaskStatistics> GetUserTaskStatistics(string userId)
        {
            try
            {
                var userTasks = await _taskService.GetTasksByUserId(userId);

                var statistics = new TaskStatistics
                {
                    TotalTasks = userTasks.Count,
                    CompletedTasks = userTasks.Count(t => t.IsCompleted),
                    PendingTasks = userTasks.Count(t => t.Status == "pending"),
                    InProgressTasks = userTasks.Count(t => t.Status == "in-progress"),
                    HighPriorityTasks = userTasks.Count(t => t.Priority == "high"),
                    MediumPriorityTasks = userTasks.Count(t => t.Priority == "medium"),
                    LowPriorityTasks = userTasks.Count(t => t.Priority == "low"),
                    OverdueTasks = userTasks.Count(t => t.DueDate.HasValue && 
                                                        t.DueDate.Value < DateTime.UtcNow && 
                                                        !t.IsCompleted)
                };

                // Calcular tasa de completitud
                if (statistics.TotalTasks > 0)
                {
                    statistics.CompletionRate = Math.Round(
                        (double)statistics.CompletedTasks / statistics.TotalTasks * 100, 2
                    );
                }

                // Tareas por prioridad
                var tasksByPriority = userTasks
                    .GroupBy(t => t.Priority)
                    .Select(g => new TasksByPriority
                    {
                        Priority = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                statistics.TasksByPriorities = tasksByPriority;

                // Tareas por estado
                var tasksByStatus = userTasks
                    .GroupBy(t => t.Status)
                    .Select(g => new TasksByStatus
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                statistics.TasksByStatuses = tasksByStatus;

                return statistics;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas del usuario: {ex.Message}");
            }
        }

        public async Task<List<TaskItem>> GetOverdueTasks()
        {
            try
            {
                var allTasks = await _taskService.GetAllTasks();
                
                return allTasks
                    .Where(t => t.DueDate.HasValue && 
                               t.DueDate.Value < DateTime.UtcNow && 
                               !t.IsCompleted)
                    .OrderBy(t => t.DueDate)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<TaskItem>();
            }
        }

        public async Task<List<TaskItem>> GetTasksDueSoon(int days = 7)
        {
            try
            {
                var allTasks = await _taskService.GetAllTasks();
                var dateLimit = DateTime.UtcNow.AddDays(days);

                return allTasks
                    .Where(t => t.DueDate.HasValue && 
                               t.DueDate.Value <= dateLimit && 
                               t.DueDate.Value >= DateTime.UtcNow &&
                               !t.IsCompleted)
                    .OrderBy(t => t.DueDate)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<TaskItem>();
            }
        }
    }
}