using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Management.Models.DatabaseModel
{
    public class User
    {
        [Key]
        [Column(TypeName = "varchar(100)")]
        public string ID { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        public string? PhotoURL { get; set; }
    }
}
