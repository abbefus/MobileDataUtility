using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class ARCHSPolygon
    {
        public ARCHSPolygon()
        {
            PolygonShp = new byte[0];
        }

        public Guid ARCHSGuid { get; set; }
        public byte[] PolygonShp { get; set; }
    }
}
