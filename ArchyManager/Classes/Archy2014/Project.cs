using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class Project
    {
        public Int16? ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectTitle { get; set; }
        public Guid ProjectGuid { get; set; }
        public string Province { get; set; }
        public string ProjectNumber { get; set; }
    }
}
