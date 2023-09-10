using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Management.Models.DatabaseModel
{
    public enum Status
    {
        NotStarted,
        InProgress,
        Completed,
        Cancelled
    }
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "varchar(100)")]
        public string? ID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; } = string.Empty;

        public Status Status { get; set; } 

        [ForeignKey("ProjectID")]
        public virtual Project? Project { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? ProjectID { get; set; }


        [ForeignKey("AssigneeID")]
        public virtual User? Assignee { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Required]
        public string? AssigneeID { get; set; }

       
        [ForeignKey("CreatorID")]
        public virtual User? Creator { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? CreatorID { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? FinishTime { get; set; }

    }
}