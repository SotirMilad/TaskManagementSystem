using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }= null!; 

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public TaskState Status { get; set; } = TaskState.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;


        public DateOnly? DueDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
