namespace Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User? User { get; set; }

    public List<TaskItem> TaskItems { get; set; } = new();

    public Project()
    {
        
    }

    public bool AddTaskItem(TaskItem taskItem)
    {
        if (TaskItems.Count >= 20)
            return false;

        TaskItems.Add(taskItem);
        return true;
    }

    public void ValidateDelete()
    {
        if (TaskItems.Any(t => t.Status == Enums.TaskStatus.Pendente))
            throw new InvalidOperationException("O projeto não pode ser deletado enquanto existir tarefas pendentes. Por favor, complete ou remova as tarefas");
    }
}