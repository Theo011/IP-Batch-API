using IP_Batch_API.DbContexts;
using IP_Batch_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IP_Batch_API.Services
{
    public class IPBatchRepository : IIPBatchRepository, IDisposable
    {
        private readonly IPBatchAPIDbContext _context;

        public IPBatchRepository(IPBatchAPIDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _context?.Dispose();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<IPDetail?> GetIPDetailsAsync(string ip)
        {
            return await _context.IPDetail.FirstOrDefaultAsync(ipDetail => ipDetail.Ip == ip);
        }

        public async Task AddIPDetails(IPDetail ipDetails)
        {
            if (ipDetails == null)
                throw new ArgumentNullException(nameof(ipDetails));

            await _context.IPDetail.AddAsync(ipDetails);
        }

        public void UpdateIPDetails(IPDetail ipDetails)
        {
            if (ipDetails == null)
                throw new ArgumentNullException(nameof(ipDetails));

            _context.IPDetail.Update(ipDetails);
        }

        public void DeleteIPDetails(IPDetail ipDetails)
        {
            if (ipDetails == null)
                throw new ArgumentNullException(nameof(ipDetails));

            _context.IPDetail.Remove(ipDetails);
        }
    }
}