using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PartnerShipProducts : BaseEntity
    {
        public string Name { get; set; }
        public string Count { get; set; }
        public Partnership Partnership { get; set; }
        public Guid PartnershipId { get; set; }
    }
}
