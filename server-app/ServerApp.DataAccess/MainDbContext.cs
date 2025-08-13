using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ServerApp.DataAccess;

public class MainDbContext : IdentityDbContext<UserProfileEntity>
{
    public MainDbContext(DbContextOptions<MainDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }    
}
