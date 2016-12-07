using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class Archy2014DB
    {
        public ARCHS[] ARCHSs { get; set; }
        public ArchSite[] ArchSites { get; set; }
        public ArchSitePhotolog[] ArchSitePhotologs { get; set; }
        public ARCHSPolygon[] ARCHSPolygons { get; set; }
        public AreaGeometry[] AreaGeometries { get; set; }
        public Project[] Projects { get; set; }
        public ShovelTestPit[] ShovelTestPits { get; set; }
    }
}
