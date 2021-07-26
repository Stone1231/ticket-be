using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Backend.Models
{
    [Table("user")]
    public class User
    {
        public User()
        {
        }
        
        [Column("id")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
        
        [Column("role")]
        public UserRole Role { get; set; }
    }
    
    
    public enum UserRole : ushort
    {
        Admin = 1,
        PM = 2,
        RD = 3,
        QA = 4
    }
}