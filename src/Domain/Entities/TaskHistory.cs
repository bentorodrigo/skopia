namespace Domain.Entities;

public class TaskHistory
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public string Modification { get; set; } = string.Empty;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
    public string ModifiedBy { get; set; } = string.Empty;

    public TaskHistory() { }
}
