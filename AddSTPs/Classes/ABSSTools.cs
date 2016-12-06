using Excel;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace ABSSTools
{
    //reads a path to Excel xls or xlsx into a DataTableCollection
    class ExcelReader
    {
        public static DataTableCollection Read(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader excelReader =
                        (Path.GetExtension(path) == ".xls") ?
                        ExcelReaderFactory.CreateBinaryReader(stream) :
                        ExcelReaderFactory.CreateOpenXmlReader(stream);

                    return excelReader.AsDataSet().Tables;
                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.Message);
                return null;
            }
            
        }
        public static string[] ExtractSheetNames(string path)
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader =
                    (Path.GetExtension(path) == ".xls") ?
                    ExcelReaderFactory.CreateBinaryReader(stream) :
                    ExcelReaderFactory.CreateOpenXmlReader(stream);

                return excelReader.AsDataSet().Tables.Cast<DataTable>().Select(x => x.TableName).ToArray();
            }
        }
    }
}
