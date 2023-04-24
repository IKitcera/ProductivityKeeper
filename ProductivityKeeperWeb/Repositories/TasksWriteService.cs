using Microsoft.AspNetCore.Authorization;
using ProductivityKeeperWeb.Data;

namespace ProductivityKeeperWeb.Repositories
{
    [Authorize]
    public class TasksWriteService
    {
        private readonly ApplicationContext _context;
        public TasksWriteService(ApplicationContext context)
        {
            _context = context;
        }
    }

    public Task<Cate>
}
