using ArchyManager.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class ARCHSPolygon : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "ARCHSGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public ARCHSPolygon()
        {
            PolygonShp = new byte[0];
        }

        public Guid ARCHSGuid { get; set; }
        public byte[] PolygonShp { get; set; }
    }
}
