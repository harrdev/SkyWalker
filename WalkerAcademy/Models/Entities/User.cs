using Microsoft.AspNetCore.Identity;

namespace WalkerAcademy.Models.Entities
{
    public class User : IdentityUser
    {
        public ICollection<VehicleEntity> Vehicles { get; set; }
        public ICollection<SpeciesEntity> Species { get; set; }
        public ICollection<PersonEntity> People { get; set; }
        public ICollection<OrganizationEntity> Organizations { get; set; }
        public ICollection<LocationEntity> Locations { get; set; }
        public ICollection<DroidEntity> Droids { get; set; }
        public ICollection<CreatureEntity> Creatures { get; set; }
    }
}
