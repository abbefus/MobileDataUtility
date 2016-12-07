using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class AreaGeometry
    {
        public AreaGeometry()
        {
            AreaGeometry1 = new byte[0];
        }
        public byte[] AreaGeometry1 { get; set; }
        public Guid AreaGuid { get; set; }
        public Guid ParentGuid { get; set; }
    }
}
