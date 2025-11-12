namespace Inv.Domain.Base
{
    public class Entity<T>
    {
        public T Id { get; set; }

        //There could be CreatedOn and ModifiedOn for tracking purposes
    }
}