using ArchyManager.Classes;
using ArchyManager.Classes.Archy2014;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager
{
    class UploadSDF
    {
        private ArchyConnection ac;

        public UploadSDF()
        {
            ac = new ArchyConnection();
        }

        private Project QueryProject(string projectnumber)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT")
                    .AppendLine("ProjectNumber")
                    .AppendLine("ProjectName")
                    .AppendLine("ProjectTitle")
                    .AppendLine("ProjectGuid")
                    .AppendLine("Province")
                    .AppendLine("ProjectNumber")
                .AppendLine("FROM [Archy2014].[dbo].[Project]")
                .AppendLine(string.Format("WHERE ProjectNumber = '{0}'", projectnumber));

            ac.OpenConnection();
            ac.dbCommand.CommandType = CommandType.Text;
            ac.dbCommand.CommandText = commandSB.ToString();
            Project project = new Project { ProjectNumber = projectnumber };

            using (SqlDataReader rdr = ac.dbCommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    project.ProjectID = rdr.SafeGetInt16("ProjectID");
                    project.ProjectName = rdr.SafeGetString("ProjectName");
                    project.ProjectTitle = rdr.SafeGetString("ProjectTitle");
                    project.ProjectGuid = rdr.SafeGetGuid("ProjectGuid");
                    project.Province = rdr.SafeGetString("Province");
                    project.ProjectNumber = rdr.SafeGetString("ProjectNumber");
                }
            }
            ac.CloseConnection();

            return project;
        }





    }

}
