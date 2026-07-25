using Microsoft.AspNetCore.Mvc;

namespace TaskManagementSystem.DTOs.Common
{
    public class TaskQueryParameters
    {
            public int Page { get; set; } = 1;

            public int Limit { get; set; } = 10;

            public string? Status { get; set; }

            public string? Priority { get; set; }

            public DateOnly? DueDateFrom { get; set; }

            public DateOnly? DueDateTo { get; set; }

            public string? SortBy { get; set; }

            public string SortDirection { get; set; } = "asc";


            [FromQuery(Name = "q")]
            public string? Search { get; set; }
    }

}