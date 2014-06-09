using System;

namespace evs.Model
{
    public class ResourceItemCategory
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public Int32 OwnerId { get; set; }
        public Boolean Active { get; set; }
    }
}
