namespace TaskManagementSystem.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public User User { get; set; } = null!;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }

}
