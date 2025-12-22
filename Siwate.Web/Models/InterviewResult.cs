using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siwate.Web.Models
{
    [Table("interview_results")]
    public class InterviewResult
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column("answer_id")]
        public Guid AnswerId { get; set; }

        [ForeignKey("AnswerId")]
        public Answer Answer { get; set; }

        [Column("score")]
        [Range(0, 100)]
        public int Score { get; set; }

        [Column("feedback")]
        public string Feedback { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
