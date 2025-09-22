using System.ComponentModel.DataAnnotations;

namespace ChatApplication.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
