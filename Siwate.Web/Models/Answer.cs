using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siwate.Web.Models
{
    [Table("answers")]
    public class Answer
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column("question_id")]
        public Guid QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        [Required]
        [Column("answer_text")]
        public string AnswerText { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
