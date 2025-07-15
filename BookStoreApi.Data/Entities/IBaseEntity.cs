namespace BookStoreApi.Entities
{
    public interface IBaseEntity
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        int Id { get; set; }
        DateTime? InactiveDate { get; set; }
        string? ArchiveReason { get; set; }
        DateTime? ModifiedAt { get; set; }
        string ModifiedBy { get; set; }
    }
}
