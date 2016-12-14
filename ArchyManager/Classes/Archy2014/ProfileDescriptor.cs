using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class ProfileDescriptor : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "RecGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Guid ProfileGuid { get; set; }
        public byte DescriptorType { get; set; }
        public byte DescriptorOrder { get; set; }
        public string Modifier { get; set; }
        public string Value { get; set; }
        public Guid RecGuid { get; set; }
        public byte? PctInclusion { get; set; }
    }
}
