using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WalkerAcademy.Models.Entities;

namespace WalkerAcademy.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<VehicleEntity> Vehicles { get; set; }
        public DbSet<SpeciesEntity> Species { get; set; }
        public DbSet<PersonEntity> People { get; set; }
        public DbSet<OrganizationEntity> Organizations { get; set; }
        public DbSet<LocationEntity> Locations { get; set; }
        public DbSet<DroidEntity> Droids { get; set; }
        public DbSet<CreatureEntity> Creatures { get; set; }

    }
}
