using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Entities
{
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime? ModifiedAt { get; set; }
        [Required]
        [MaxLength(50)]
        public string CreatedBy { get; set; }
        [MaxLength(50)]
        public string ModifiedBy { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? InactiveDate { get; set; }
        public string? ArchiveReason { get; set; }
    }
}
