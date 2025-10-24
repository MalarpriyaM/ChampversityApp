using Champversity.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Champversity.DataAccess
{
    public interface IManualTaskService
  {
     Task<List<ManualTask>> GetPendingTasksAsync();
     Task<List<ManualTask>> GetTasksByAssigneeAsync(string assignee);
        Task<ManualTask> GetTaskByIdAsync(int taskId);
     Task<bool> UpdateTaskStatusAsync(int taskId, string status, string notes);
   Task<bool> CompleteTaskAsync(int taskId, string completionNotes);
 Task<bool> AssignTaskAsync(int taskId, string assignee);
     Task<ManualTask> CreateTaskAsync(int studentId, string taskType, string description, string priority = "Medium");
    }

    public class ManualTaskService : IManualTaskService
    {
        private readonly ApplicationDbContext _dbContext;

   public ManualTaskService(ApplicationDbContext dbContext)
     {
   _dbContext = dbContext;
     }

  public async Task<List<ManualTask>> GetPendingTasksAsync()
        {
          return await _dbContext.ManualTasks
      .Where(t => t.Status == "Pending" || t.Status == "InProgress")
    .OrderByDescending(t => t.Priority == "High" ? 3 : t.Priority == "Medium" ? 2 : 1)
      .ThenBy(t => t.CreatedDate)
    .ToListAsync();
        }

 public async Task<List<ManualTask>> GetTasksByAssigneeAsync(string assignee)
      {
    return await _dbContext.ManualTasks
   .Where(t => t.AssignedTo == assignee && (t.Status == "Pending" || t.Status == "InProgress"))
  .OrderByDescending(t => t.Priority == "High" ? 3 : t.Priority == "Medium" ? 2 : 1)
  .ThenBy(t => t.CreatedDate)
        .ToListAsync();
 }

  public async Task<ManualTask> GetTaskByIdAsync(int taskId)
   {
  return await _dbContext.ManualTasks.FindAsync(taskId);
 }

     public async Task<bool> UpdateTaskStatusAsync(int taskId, string status, string notes)
        {
        try
      {
   var task = await _dbContext.ManualTasks.FindAsync(taskId);
       if (task == null) return false;

     task.Status = status;
      if (!string.IsNullOrEmpty(notes))
  {
       task.Notes = string.IsNullOrEmpty(task.Notes) ? notes : $"{task.Notes}\n{DateTime.Now:yyyy-MM-dd HH:mm}: {notes}";
        }

      if (status == "Completed")
      {
 task.CompletedDate = DateTime.Now;
        }

     await _dbContext.SaveChangesAsync();
          return true;
        }
 catch
   {
      return false;
     }
   }

     public async Task<bool> CompleteTaskAsync(int taskId, string completionNotes)
 {
    return await UpdateTaskStatusAsync(taskId, "Completed", completionNotes);
   }

 public async Task<bool> AssignTaskAsync(int taskId, string assignee)
        {
   try
        {
    var task = await _dbContext.ManualTasks.FindAsync(taskId);
    if (task == null) return false;

    task.AssignedTo = assignee;
       if (task.Status == "Pending")
       {
        task.Status = "InProgress";
      }

      await _dbContext.SaveChangesAsync();
    return true;
      }
      catch
        {
      return false;
        }
    }

        public async Task<ManualTask> CreateTaskAsync(int studentId, string taskType, string description, string priority = "Medium")
        {
     try
      {
         var task = new ManualTask
     {
  StudentId = studentId,
        TaskType = taskType,
        TaskDescription = description,
      Priority = priority,
      Status = "Pending",
          CreatedDate = DateTime.Now,
         AssignedTo = "Unassigned"
    };

     _dbContext.ManualTasks.Add(task);
      await _dbContext.SaveChangesAsync();
         return task;
        }
    catch
    {
        return null;
}
        }
    }
}