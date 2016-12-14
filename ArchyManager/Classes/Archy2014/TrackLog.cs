using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class TrackLog : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "TrackGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public TrackLog()
        {
            Track = new byte[0];
        }
        public Int16? ProjectID { get; set; }
        public string Recorder { get; set; }
        public DateTime? CreateTime { get; set; }
        public byte[] Track { get; set; }
        public Guid TrackGuid { get; set; }
        public Int32? PermitID { get; set; }
    }
}
