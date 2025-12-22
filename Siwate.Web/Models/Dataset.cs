using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siwate.Web.Models
{
    [Table("datasets")]
    public class Dataset
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("answer_text")]
        public string AnswerText { get; set; }

        [Column("score")]
        [Range(0, 100)]
        public int Score { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
