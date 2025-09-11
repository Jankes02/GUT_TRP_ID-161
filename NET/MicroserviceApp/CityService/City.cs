using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityService
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