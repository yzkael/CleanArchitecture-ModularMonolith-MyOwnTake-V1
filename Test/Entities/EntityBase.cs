using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Entities
{
    public class EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastUpdateAt { get; private set; } = DateTimeOffset.UtcNow;

        public void SetLastUpdatedAt()
        {
            LastUpdateAt = DateTimeOffset.UtcNow;
        }
    }
}