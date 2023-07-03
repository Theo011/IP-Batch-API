using IP_Batch_API.Entities;

namespace IP_Batch_API.Services
{
    public interface IIPBatchRepository
    {
        Task<bool> SaveChangesAsync();
        Task<IPDetail?> GetIPDetailsAsync(string ip);
        Task AddIPDetails(IPDetail ipDetails);
        void UpdateIPDetails(IPDetail ipDetails);
        void DeleteIPDetails(IPDetail ipDetails);
    }
}