using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Backend.Models
{
    [Table("ticket")]
    public class Ticket
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        
        [Column("summary")]
        public string Summary { get; set; }
        
        [Column("description")]
        public string Description { get; set; }
        
        [Column("ticket_type")]
        public TicketType Type { get; set; }
        
        [Column("status")]
        public StatusType Status { get; set; }
        
        [Column("severity_level")]
        public SeverityLevel Level { get; set; }
        
        [Column("update_time")]
        public DateTime Update { get; set; }
        
        [JsonProperty(PropertyName = "user_id")] 
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        // [JsonIgnore]
        public virtual User User { get; set; }
    }
    
    public enum TicketType : ushort
    {
        Feature = 1,
        Bug = 2,
        TestCase = 3
    }
    
    public enum StatusType : ushort
    {
        Start = 0,
        Finish = 1
    }
    
    public enum SeverityLevel : ushort
    {
        Critical = 1,
        High = 2,
        Medium = 3,
        Low = 4
    }
}