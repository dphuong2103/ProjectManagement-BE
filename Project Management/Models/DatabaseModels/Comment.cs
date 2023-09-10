using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Management.Models.DatabaseModel
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "varchar(100)")]
        public string? ID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; }

        [ForeignKey("Task")]
        [Column(TypeName = "varchar(100)")]
        [Required]
        public string TaskID { get; set; }

        public virtual Task? Task { get; set; }

        [ForeignKey("CreatorID")]
        public virtual User? Creator { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? CreatorID { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    }
}