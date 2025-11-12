namespace Inv.Domain.Base
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
    public class SoftDeleteBase<T> : Entity<T>, ISoftDelete
    {
        public bool IsDeleted { get; set; }
    }
}