using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonolithicApp.Database.Model
{
    public class Connection
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Column("from_city_id")]
        public string FromCityId { get; set; }

        [Column("to_city_id")]
        public string ToCityId { get; set; }

        [Column("valid_from")]
        public DateTime ValidFrom { get; set; }

        [Column("valid_to")]
        public DateTime ValidTo { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("FromCityId")]
        public City FromCity { get; set; }

        [ForeignKey("ToCityId")]
        public City ToCity { get; set; }
    }
}
