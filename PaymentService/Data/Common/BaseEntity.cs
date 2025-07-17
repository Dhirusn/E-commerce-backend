using System.ComponentModel.DataAnnotations;

namespace PaymentService.Data.Common
{
    public abstract class BaseEntity<TKey> where TKey : struct
    {
        [Key]
        public TKey Id { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        public DateTime? DeletedAt { get; set; }
    }
}
