using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class STPProfile : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "ProfileGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Guid STPGuid { get; set; }
        public Int16? UpperDepth { get; set; }
        public Int16? LowerDepth { get; set; }
        public bool CulturalMaterial { get; set; }
        public string ProfileNote { get; set; }
        public Guid ProfileGuid { get; set; }
    }
}
