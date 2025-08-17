// Infrastructure/Auth/AuthDbContext.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Data.Auth
{
    public class AuthDbContext : IdentityDbContext<
        IdentityUser<Guid>,
        IdentityRole<Guid>,
        Guid>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    }
}