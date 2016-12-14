using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class CrewDefinition : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "CrewMemberGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Guid IAGuid { get; set; }
        public string MemberName { get; set; }
        public string Affiliation { get; set; }
        public byte? RoleID { get; set; }
        public bool? OnSite { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public Guid? CrewMemberGuid { get; set; }
        public bool? Recorder { get; set; }
    }
}
