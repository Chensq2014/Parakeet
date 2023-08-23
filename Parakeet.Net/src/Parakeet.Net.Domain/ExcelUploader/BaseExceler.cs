using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Parakeet.Net.Dtos;
using Parakeet.Net.Entities;
using Parakeet.Net.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Parakeet.Net.ExcelUploader
{
    /// <summary>
    /// Excel数据转换抽象基类 
    /// <typeparam name="TEntity">导入数据类型</typeparam>
    /// </summary>
    public abstract class BaseExceler<TEntity> where TEntity : BaseEntity, new()
    {
        #region 保护属性 子类继承

        /// <summary>
        /// 最小数据列长度，用于导入验证，子类Validate时赋值
        /// </summary>
        protected int MinColumnCount { get; set; }

        /// <summary>
        /// 错误消息 提示：第几行什么错(字符串)
        /// </summary>
        protected List<string> ErrorMessages { get; set; } = new List<string>();

        #endregion

        #region 加载excel文件到内存 仅给子类继承的加载excel文件到内存获取Sheet方法 不可重写

        /// <summary>
        /// 加载Workbook
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IWorkbook> LoadBook(string path)
        {
            Stream fs = null;
            IWorkbook book;
            try
            {
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }
                else
                {
                    CheckError("不存在此文件");
                }

                if (path.EndsWith(".xlsx"))
                {
                    book = new XSSFWorkbook(fs);
                }
                else if (path.EndsWith(".xls"))
                {
                    book = new HSSFWorkbook(fs);
                }
                else
                {
                    throw new Exception($"上传文件类型出错:只允许上传.xlsx/.xls文件");
                }
                return await Task.FromResult(book);
            }
            finally
            {
                fs?.Dispose();
                fs?.Close();
            }
        }

        /// <summary>
        /// 加载文件重载1
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheetNum"></param>
        /// <returns></returns>
        protected ISheet LoadFile(string path, int sheetNum = 0)
        {
            if (!File.Exists(path))
            {
                CheckError("不存在此文件");
            }
            var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
            IWorkbook book;
            if (path.EndsWith(".xlsx"))
            {
                book = new XSSFWorkbook(fs);
            }
            else if (path.EndsWith(".xls"))
            {
                book = new HSSFWorkbook(fs);
            }
            else
            {
                throw new UserFriendlyException("上传文件类型出错");
            }

            var sheet = book.GetSheetAt(sheetNum);

            if (sheet.GetRow(1) == null)
            {
                CheckError("不存在数据");
            }

            return sheet;
        }

        /// <summary>
        /// 加载文件重载2
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sheetNum"></param>
        /// <returns></returns>
        protected ISheet LoadFile(IFormFile file, int sheetNum = 0)
        {
            // 100M以内
            var maxFileSize = 1024 * 1024 * 100;
            // 判断文件是否是指定文件
            if (file == null || file.Length <= 0)
            {
                CheckError("导入的文件格式不对");
            }
            // 验证文件大小
            if (file.Length > maxFileSize)
            {
                CheckError("文件不能大于100M");
            }
            // 判断文件是xlsx
            if (!file.FileName.Contains(".xlsx"))
            {
                CheckError("上传文件类型出错");
            }

            IWorkbook book = new XSSFWorkbook(file.OpenReadStream());
            ISheet sheet = book.GetSheetAt(sheetNum);
            if (sheet.GetRow(1) == null)
            {
                CheckError("不存在数据");
            }
            return sheet;
        }

        #endregion

        #region 有默认实现且开放扩展成员函数--读取单行数据到实体/实体集

        /// <summary>
        /// 泛型方法 不带时间轴 读取sheet单行实体
        /// </summary>
        /// <param name="input">读sheet表单参数</param>
        /// <returns></returns>
        public virtual List<TEntity> ReadDataRowToEntities(ReadExcelToEntityDto input)
        {
            if (input.Sheet is null)
            {
                input.Sheet = input.Path.HasValue() ? LoadFile(input.Path) : LoadFile(input.File);
            }
            if (!Validate(input.Sheet.GetRow(0)))
            {
                throw new Exception("此数据模板格式不符合导出格式!");
            }
            var entities = new List<TEntity>();
            for (var rowIndex = input.StartRowIndex; rowIndex <= input.Sheet.LastRowNum - input.Skip; rowIndex++)
            {
                var dataRow = input.Sheet.GetRow(rowIndex);
                if (dataRow == null || dataRow.LastCellNum + 1 < MinColumnCount || dataRow.GetCell(0) != dataRow.Cells[0])
                {
                    //|| dataRow.Cells.Any(m => m.CellType == CellType.Blank) //替换dataRow.Cells.Count 为dataRow.LastCellNum + 1
                    break;
                }
                var cells = dataRow.Cells;//如果前面有为空的数据会跳过导致数据错位所以判断一下
                if (cells[0] != null && cells[0].CellType != CellType.Blank
                    || cells[0].IsMergedCell && GetMergeColumnValue(input.Sheet, cells[0]) != null)
                {
                    entities.Add(ExcelDataRowToEntities(input.StartColumnIndex, dataRow));
                }
            }
            return entities;
        }

        /// <summary>
        /// 具体exceldata转换为entities留给子类扩展 一行一个实体
        /// </summary>
        /// <param name="dataRow">数据行</param>
        /// <param name="columnIndex">循环行的列下标</param>
        /// <returns></returns>
        public virtual TEntity ExcelDataRowToEntities(int columnIndex, IRow dataRow)
        {
            var dataRowCells = dataRow.Cells;//子类根据这一行所有列的下标取值赋值给实体对象
            var entity = new TEntity();
            try
            {
                entity.SetEntityPrimaryKey(Guid.Parse(dataRowCells[columnIndex++].StringCellValue));
                //注释部分代码提供给子类重写时参考
                //entity.BusinessProductId = (int?)dataRowCells[0].NumericCellValue;
                //entity.Amount=(decimal)dataRowCells[index++].NumericCellValue,//(decimal)GetCellValueOrNull(cells[index++]);
            }
            catch (Exception e)
            {
                ErrorMessages.Add($"第{dataRow.RowNum}行第{columnIndex}列数据转换错误：{e.Message}");//如果有错误，直接往里面添加，统一抛出
            }
            //ErrorMessages.Add();//如果有错误，直接往里面添加，统一抛出
            return entity;
        }

        /// <summary>
        /// 泛型方法 带时间轴
        /// </summary>
        /// <param name="input">读取数据到实体对象</param>
        /// <returns></returns>
        public virtual List<TEntity> ReadDataLineToEntities(ReadExcelToEntityDto input)
        {
            if (input.Sheet is null)
            {
                input.Sheet = input.Path.HasValue() ? LoadFile(input.Path) : LoadFile(input.File);
            }
            if (!Validate(input.Sheet.GetRow(0)))
            {
                CheckError("此数据模板格式不符合导出格式!");
            }
            var entities = new List<TEntity>();
            var dateRow = input.Sheet.GetRow(input.DateRowIndex);
            for (var rowIndex = input.StartRowIndex; rowIndex <= input.Sheet.LastRowNum - input.Skip; rowIndex++)
            {
                var dataRow = input.Sheet.GetRow(rowIndex);//dataRow.Cells;如果前面有为空的数据会跳过导致数据错位 所以做判断
                if (dataRow == null || dataRow.LastCellNum + 1 < MinColumnCount || dataRow.GetCell(0) != dataRow.Cells[0])
                {
                    //|| dataRow.Cells.Any(m => m.CellType == CellType.Blank)
                    //替换dataRow.Cells.Count 为dataRow.LastCellNum + 1
                    break;
                }
                entities.AddRange(ExcelDataLineToEntities(input.StartColumnIndex, dataRow, dateRow));
            }
            return entities;
        }

        /// <summary>
        /// 具体exceldata转换为entities留给子类扩展
        /// </summary>
        /// <param name="startColumnIndex">循环列的开始</param>
        /// <param name="dataRow">数据列</param>
        /// <param name="dateRow">日期数据行</param>
        /// <returns></returns>
        public virtual List<TEntity> ExcelDataLineToEntities(int startColumnIndex, IRow dataRow, IRow dateRow)
        {
            var list = new List<TEntity>();
            var cells = dataRow.Cells;//如果前面有为空的数据会跳过导致数据错位 所以判断一下
            if (cells[0] != null && cells[0].CellType != CellType.Blank
                || cells[0].IsMergedCell && GetMergeColumnValue(dataRow.Sheet, cells[0]) != null)
            {
                for (var i = startColumnIndex; i < dataRow.LastCellNum; i++)
                {
                    try
                    {
                        list.Add(new TEntity
                        {
                            //注释部分代码提供给子类重写时参考
                            //Id = Guid.Parse(dataRow.GetCell(i).StringCellValue),//inex
                            //BusinessProductId = (int?)dataRowCells[0].NumericCellValue,
                            //Amount = (decimal)dataRowCells[i].NumericCellValue,//(decimal)GetCellValueOrNull(cells[i]),
                            //Time = (dateRow.Cells[i].CellType == CellType.Numeric
                            //    ? dateRow.Cells[i].DateCellValue
                            //    : DateTime.Parse(dateRow.Cells[i].StringCellValue)).ToMonthDate(),
                        });
                    }
                    catch (Exception e)
                    {
                        ErrorMessages.Add($"第{dataRow.RowNum}行第{i}列数据转换错误：{e.Message}");//如果有错误，直接往里面添加，统一抛出
                    }
                }
            }
            CheckError();//把整个excel里面报错信息全部统一输出
            return list;
        }

        #endregion

        #region 检查错误，获取cell数值数据转换，合并行列数据转换 提供默认实现，子类可重写的公共成员函数(多态)

        /// <summary>
        ///  检查抛出友好错误消息
        /// </summary>
        /// <param name="error"></param>
        public virtual void CheckError(string error = null)
        {
            if (error.HasValue())
            {
                ErrorMessages.Add(error);
            }
            if (ErrorMessages.Any())
            {
                throw new UserFriendlyException(string.Join('\n', ErrorMessages));
            }
        }

        /// <summary>
        /// 判断是否为空返回double值
        /// </summary>
        /// <param name="cell">cell</param>
        /// <returns></returns>
        public virtual double GetCellValueOrZero(ICell cell)
        {
            if (cell.CellType == CellType.Numeric || cell.CellType == CellType.Formula)
            {
                return cell.NumericCellValue;
            }
            return cell.StringCellValue.HasValue() ? cell.NumericCellValue : 0;
        }

        /// <summary>
        /// MergeColumn 合并列的取值
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="nowcell"></param>
        /// <returns></returns>
        public virtual ICell GetMergeColumnValue(ISheet sheet, ICell nowcell)
        {
            for (var mergeIndex = 0; mergeIndex < sheet.NumMergedRegions; mergeIndex++)
            {
                var cellrange = sheet.GetMergedRegion(mergeIndex);
                if (nowcell.ColumnIndex >= cellrange.FirstColumn && nowcell.ColumnIndex <= cellrange.LastColumn
                                                                 && nowcell.RowIndex >= cellrange.FirstRow && nowcell.RowIndex <= cellrange.LastRow)
                {
                    //这里是cell所在的合并单元格
                    //nowcell = sheet.GetRow(nowcell.RowIndex).GetCell(cellrange.FirstColumn);
                    nowcell = sheet.GetRow(cellrange.FirstRow).GetCell(cellrange.FirstColumn);
                    break;
                }
            }
            return nowcell;
        }

        #endregion

        #region excel模板验证规则 抽象成员函数 具体子类一定要实现(多态) 

        /// <summary>
        /// 验证Excel  每个子类的格式验证都有自己的模板规则 下标都是0开始
        /// </summary>
        /// <param name="row">传入首行(Header行)/需要验证的行对象</param>
        /// <returns></returns>
        public abstract bool Validate(IRow row);

        /// <summary>
        /// 导出Excel 暂不重要但保留(因为前端控件一般能直接能导出excel)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract Task<FileStream> Export(IQueryable<TEntity> data);

        ///// <summary>
        ///// 异步导出Excel模板
        ///// </summary>
        ///// <returns>返回导出文件key</returns>
        //public abstract Task<string> ExportTemplateAsync();

        #endregion
    }
}
