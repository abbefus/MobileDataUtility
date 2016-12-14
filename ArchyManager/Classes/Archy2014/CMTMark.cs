using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class CMTMark : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "CMTMarkGuid"; } }
        [Browsable(false)]

        public bool IsUploaded { get; set; }
        public Guid CMTGuid { get; set; }
        public Guid CMTMarkGuid { get; set; }
        public byte? ScarTypeShapeID { get; set; }
        public byte? ScarTopShape { get; set; }
        public byte? MarkNumber { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        public double? HtAboveGround { get; set; }
        public byte? SideID { get; set; }
        public bool? Toolmark { get; set; }
        public byte? ToolmarkTypeID { get; set; }
        public byte? SlopeID { get; set; }
        public byte? CMTPre1846 { get; set; }
        public Int16? ScarAge { get; set; }
        public string PhotoRange { get; set; }
        public byte? CoreTypeID { get; set; }
        public string Post1846Justification { get; set; }
        public byte? MarkClassID { get; set; }
    }
}
