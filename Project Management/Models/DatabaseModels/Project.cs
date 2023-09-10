using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Project_Management.Models.DatabaseModel
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "varchar(100)")]
        public string? ID { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "varchar(100)")]
        public String? LeaderID { get; set; }

        [ForeignKey("LeaderID")]
        public virtual User? Leader { get; set; }

        [Column(TypeName = "varchar(100)")]
        public String? CreatorID { get; set; }

        [ForeignKey("CreatorID")]
        public virtual User? Creator { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }

}