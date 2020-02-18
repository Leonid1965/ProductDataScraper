using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDataScraper.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(50)]
        [Key]
        public string Email { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
