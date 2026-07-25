namespace TaskManagementSystem.DTOs.Tasks.Requests
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Status { get; set; }

        public string? Priority { get; set; }

        public DateOnly? DueDate { get; set; }
    }
}
