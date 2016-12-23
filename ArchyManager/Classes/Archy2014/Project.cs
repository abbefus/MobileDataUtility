using ArchyManager.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    public class Project : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "ProjectGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        [Browsable(true)]
        public Int16 ProjectID { get; set; }
        [Browsable(true)]
        public string ProjectName { get; set; }
        [Browsable(true)]
        public string ProjectTitle { get; set; }
        [Browsable(true)]
        public Guid ProjectGuid { get; set; }
        [Browsable(true)]
        public string Province { get; set; }
        [Browsable(true)]
        public string ProjectNumber { get; set; }
    }
}
