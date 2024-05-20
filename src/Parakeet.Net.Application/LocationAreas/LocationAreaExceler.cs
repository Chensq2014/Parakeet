using Common.Entities;
using Common.Enums;
using Common.ExcelUploader;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.LocationAreas
{
    /// <summary>
    /// 导入区域数据服务
    /// </summary>
    public class LocationAreaExceler : BaseExceler<LocationArea>, ITransientDependency
    {
        /// <summary>
        /// 1行对应1个实体
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public override LocationArea ExcelDataRowToEntities(int columnIndex, IRow dataRow)
        {
            var cells = dataRow.Cells;//子类根据这一行所有列的下标取值赋值给实体对象
            var entity = new LocationArea(Guid.Parse(cells[columnIndex++].StringCellValue));
            try
            {
                //entity.SetEntityPrimaryKey(Guid.Parse(cells[columnIndex++].StringCellValue));
                entity.ParentId = Guid.Parse(cells[columnIndex++].StringCellValue);
                entity.Code = cells[columnIndex++].StringCellValue;
                entity.ParentCode = cells[columnIndex++].StringCellValue;
                entity.Level = cells[columnIndex++].CellType == CellType.Numeric
                    ? ((int)cells[columnIndex].NumericCellValue).ToEnum<DeepLevelType>()
                    : ((int?)GetCellValueOrZero(cells[columnIndex]) ?? 0).ToEnum<DeepLevelType>();
                entity.Name = cells[columnIndex++].StringCellValue;
                entity.ShortName = cells[columnIndex++].StringCellValue;
                entity.FuallName = cells[columnIndex++].StringCellValue;
                entity.InternationalName = cells[columnIndex++].StringCellValue;
                entity.Pinyin = cells[columnIndex++].StringCellValue;
                entity.ZipCode = cells[columnIndex++].StringCellValue;
                entity.Latitude = (decimal)cells[columnIndex++].NumericCellValue;
                entity.Longitude = (decimal)GetCellValueOrZero(cells[columnIndex++]);
                //entity.CreationTime = (cells[columnIndex++].CellType == CellType.Numeric
                //    ? cells[columnIndex].DateCellValue
                //    : DateTime.Parse(cells[columnIndex].StringCellValue)).ToMonthDate();
            }
            catch (Exception e)
            {
                ErrorMessages.Add($"第{dataRow.RowNum}行第{columnIndex}列数据转换错误：{e.Message}");
            }
            CheckError();//如果有错误，统一抛出
            return entity;
        }

        /// <summary>
        /// 验证Excel规则 自定义
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public override bool Validate(IRow row)
        {
            MinColumnCount = 12;//设置数据列最小值12，下标11 每个导入模块此值不同，所以导入前验证时设置
            return row.GetCell(0) != null && row.GetCell(1) != null
                                          && row.GetCell(1).StringCellValue.Contains("Id")
                                          && row.GetCell(0).StringCellValue.Contains("ParentId")
                                          && row.GetCell(3).StringCellValue.Contains("Code")
                                          && row.GetCell(2).StringCellValue.Contains("ParentCode");
        }

        /// <summary>
        /// 数据源导出为文件流
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<FileStream> Export(IQueryable<LocationArea> data)
        {
            await data.ToListAsync();

            throw new NotImplementedException();
        }

    }
}
