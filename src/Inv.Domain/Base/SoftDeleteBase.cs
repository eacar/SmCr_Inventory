namespace Inv.Domain.Base
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
    public class SoftDeleteBase<T> : EntityBase<T>, ISoftDelete where T : struct
    {
        public bool IsDeleted { get; set; }
    }
}