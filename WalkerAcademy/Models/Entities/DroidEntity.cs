using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WalkerAcademy.Models.WebApiModel;

namespace WalkerAcademy.Models.Entities
{
    public class DroidEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public Droid ToApiModel()
        {
            return new Droid
            {
                Id = this.Id.ToString(),
                Name = this.Name,
                Description = this.Description,
                Image = this.Image
            };
        }
    }
}
