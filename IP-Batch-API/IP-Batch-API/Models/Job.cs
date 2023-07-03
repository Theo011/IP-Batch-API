namespace IP_Batch_API.Models
{
    public class Job
    {
        public Guid Id { get; set; }
        public List<string> Ips { get; set; } = null!;
        public int TotalItems { get; set; }
        public int ProcessedItems { get; set; }
    }
}