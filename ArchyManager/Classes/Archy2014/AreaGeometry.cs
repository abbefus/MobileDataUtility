using ArchyManager.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class AreaGeometry : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "AreaGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public AreaGeometry()
        {
            AreaGeometry_ = new byte[0];
        }
        public byte[] AreaGeometry_ { get; set; }
        public Guid AreaGuid { get; set; }
        public Guid ParentGuid { get; set; }
    }
}
