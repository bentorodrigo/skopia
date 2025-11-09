using Domain.Enums;

namespace Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pendente;
    public TaskPriority Priority { get; private set; }

    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public List<TaskHistory> Historic { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();

    public TaskItem(TaskPriority priority)
    {
        Priority = priority;
    }

    public TaskItem() { } 
}
