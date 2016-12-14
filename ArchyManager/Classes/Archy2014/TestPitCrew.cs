using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class TestPitCrew : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "TestPitCrewGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Guid TestPitGuid { get; set; }
        public Guid CrewGuid { get; set; }
        public Guid TestPitCrewGuid { get; set; }
    }
}
