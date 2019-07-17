using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rabbithole
{
    public class Event
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("timestamp")]
        public DateTime Recieved { get; set; }

        [Column("exchange")]
        public string Exchange { get; set; }

        [Column("routing_key")]
        public string RoutingKey { get; set; }
        
        [Column("content", TypeName="jsonb")]
        public string Content { get; set; }
    }
}