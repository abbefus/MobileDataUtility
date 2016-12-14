using ArchyManager.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class ArchSitePhotolog : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "PhotologGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Guid? PhotologGuid { get; set; }
        public string PhotoRange { get; set; }
        public string PhotologNote { get; set; }
        public DateTime? PhotologDate { get; set; }
        public Guid? ArchSiteGuid { get; set; }
    }
}
