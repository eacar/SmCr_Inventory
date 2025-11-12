namespace Inv.Domain.Base
{
    public interface IModified
    {
        DateTime? ModifiedOn { get; set; }
    }
    public class Entity<T> where T : struct
    {
        public T Id { get; set; }
    }
    public class EntityBase<T> : Entity<T>, IModified where T : struct
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }
    }
}