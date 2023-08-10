using System;

namespace Entities
{
    public interface IEntity
    {
    }

    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
    }

    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }

        public BaseEntity()
        {
            Active = true;
            CreationDateTime = DateTime.Now;
        }

        public DateTime LastModifiedDateTime { get; set; }
        public DateTime CreationDateTime { get; set; }
        public bool Active { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {
    }
}
