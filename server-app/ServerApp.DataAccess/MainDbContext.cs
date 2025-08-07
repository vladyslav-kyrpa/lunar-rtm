using Microsoft.EntityFrameworkCore;

namespace ServerApp.DataAccess;

public class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options) :
        base(options) { }
}
