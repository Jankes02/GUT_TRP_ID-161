using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RouteService.Database.Model
{
    public class City
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("state")]
        public string State { get; set; }

        [Column("population")]
        public int Population { get; set; }
    }
}
