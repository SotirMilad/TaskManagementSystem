namespace TaskManagementSystem.DTOs.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = new List<T>();

        public int Page { get; set; }

        public int Limit { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages
        {
            get
            {
                return Limit <= 0
                    ? 0
                    : (int)Math.Ceiling((double)TotalCount / Limit);
            }
        }
    }
}
