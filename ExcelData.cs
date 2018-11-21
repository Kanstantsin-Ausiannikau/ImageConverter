using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace ImageConverter
{
    class ExcelData
    {
        static ExcelWorksheet worksheet;

        static ExcelPackage package;

        public static List<string> GetDataFromExcel(string fileName)
        {
            FileInfo existingFile = new FileInfo(fileName);

            List<string> links = new List<string>();

            package = new ExcelPackage(existingFile);
            
            worksheet = package.Workbook.Worksheets[1];

            links = ReadFromFullExcelFile(worksheet);

            return links;
        }

        public static void SaveDataToExcel(List<bool> checkedRows, List<string> links)
        {
            if (worksheet!=null)
            {
                for (int i = 2; i <= worksheet.Dimension.Rows; i++)
                {
                    if (checkedRows.Count-1== i - 2)
                    {
                        break;
                    }
                    if (checkedRows[i-2])
                    {
                        if (links[i - 2] != null)
                        {
                            worksheet.Cells[i, 18].Value = links[i - 2];
                        }
                    }
                }
            }
            package.Save();
        }

        private static List<string> ReadFromFullExcelFile(ExcelWorksheet worksheet)
        {
            List<string> links = new List<string>();

            for (int i = 2; i <= worksheet.Dimension.Rows; i++)
            {
                if (worksheet.Cells[i, 2].Value == null)
                {
                    continue;
                }
                links.Add((string)worksheet.Cells[i, 18].Value);
            }
            return links;
        }
    }
}
