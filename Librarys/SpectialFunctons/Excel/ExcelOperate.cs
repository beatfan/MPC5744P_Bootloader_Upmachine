using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;

namespace SpecialFunctions.Excel
{
    /// <summary>
    /// 操作excel
    /// </summary>
    public class ExcelOperate
    {
        /// <summary>
        /// 根据路径，从excel中读取对应的数据到DataTable中，注意应是xls格式的excel
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public DataTable ReadFromExcel(string excelPath, bool firstRowIsTitle=true)
        {
            //定义要返回的datatable对象
            DataTable data = new DataTable();

            //excel工作表
            ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                if (!File.Exists(excelPath))
                {
                    return null;
                }
                //根据指定路径读取文件
                FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
                //根据文件流创建excel数据结构
                IWorkbook workbook = new HSSFWorkbook(fs);
                //IWorkbook workbook = new HSSFWorkbook(fs);
                //如果有指定工作表名称
                //sheet = workbook.GetSheet(sheetName);
                //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                sheet = workbook.GetSheetAt(0);
                
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    

                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        ICell cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue;
                            if (cellValue != null)
                            {
                                DataColumn column = new DataColumn(cellValue);
                                data.Columns.Add(column);
                            }
                        }
                    }
                    startRow = firstRowIsTitle ? (sheet.FirstRowNum + 1) : sheet.FirstRowNum; //如果第一行是标题列名，且不需要，则+1，从第二行开始

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　

                        if (string.IsNullOrEmpty(row.Cells[0].ToString())) continue; //第一列为空的不要

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch(Exception ex)
            {
                
                return null;
            }
        }

        /// <summary>
        /// 将datatable的数据存储到excel，excel后缀为xls
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public bool SaveToExcel(DataTable dt, string excelPath)
        {
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    workbook = new HSSFWorkbook();
                    sheet = workbook.CreateSheet("Sheet1");//创建一个名称为Sheet0的表
                    int rowCount = dt.Rows.Count;//行数
                    int columnCount = dt.Columns.Count;//列数

                    //设置列头
                    row = sheet.CreateRow(0);//excel第一行设为列头
                    for (int c = 0; c < columnCount; c++)
                    {
                        cell = row.CreateCell(c);
                        cell.SetCellValue(dt.Columns[c].ColumnName);
                    }

                    //设置每行每列的单元格,
                    for (int i = 0; i < rowCount; i++)
                    {
                        row = sheet.CreateRow(i + 1);
                        for (int j = 0; j < columnCount; j++)
                        {
                            cell = row.CreateCell(j);//excel第二行开始写入数据
                            //if(dt.Rows[i][j] is string)
                                cell.SetCellValue(dt.Rows[i][j].ToString());
                            //else
                            //    cell.SetCellValue(Convert.ToDouble(dt.Rows[i][j]));
                                
                        }
                    }
                    using (fs = File.OpenWrite(excelPath))
                    {
                        workbook.Write(fs);//向打开的这个xls文件中写入数据
                        result = true;
                    }
                }
                return result;
            }
            catch
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return false;
            }

        }
    }
}
