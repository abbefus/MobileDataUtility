using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class ArchSitePhotolog
    {
        public Guid PhotologGuid { get; set; }
        public string PhotoRange { get; set; }
        public string PhotologNote { get; set; }
        public DateTime? PhotologDate { get; set; }
        public Guid ArchSiteGuid { get; set; }
    }
}
