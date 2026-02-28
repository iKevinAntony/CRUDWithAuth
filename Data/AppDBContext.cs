using CRUDWithAuth.Models.Auth;
using CRUDWithAuth.Models.AuthRoles;
using Microsoft.EntityFrameworkCore;

namespace CRUDWithAuth.Data
{
    /// <summary>
    /// Application db operations
    /// </summary>
    public class AppDBContext : DbContext
    {
        /// <summary>
        /// Initialize AppDBContext with database options passed to base class.
        /// </summary>
        /// <param name="options"></param>
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        public DbSet<UserLogin> UserLogin { get; set; }
        public DbSet<UsersToken> UsersToken { get; set; }
    }
}
