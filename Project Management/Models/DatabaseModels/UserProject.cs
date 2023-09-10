using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Management.Models.DatabaseModel
{
    public class UserProject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "varchar(100)")]
        public string? ID { get; set; }


        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
        [Required]
        public string? UserID { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? ProjectID { get; set; }

        [ForeignKey("ProjectID")]
        public virtual Project? Project { get; set; }

        public bool? IsFavorite { get; set; }

    }
}
