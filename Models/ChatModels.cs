using System.ComponentModel.DataAnnotations;

namespace ChatApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;
    }

    public class ChatMessage
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }  // Foreign key to User
        
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}