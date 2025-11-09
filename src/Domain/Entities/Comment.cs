namespace Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public string Author { get; set; } = string.Empty;

    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public Comment() { }
}