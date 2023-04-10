using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Parakeet.Net.CustomAttributes;
using Parakeet.Net.Entities;

namespace Parakeet.Net.ExcelUploader
{
    public class ExcelHelper<TEntity> where TEntity : BaseEntity,new()
    {
        public static List<TEntity> Read(string path)
        {
            var type = typeof(BaseExceler<>);
            var modelType = typeof(TEntity);
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(m => type.IsAssignableFrom(m));
            var targetType = types.FirstOrDefault(n => n.Name.Contains(modelType.Name));
            if (targetType == null)
            {
                throw new Exception("不存在模型为" + modelType.Name + "的ExcelHelper");
            }

            var target = targetType.Assembly.CreateInstance(targetType.FullName, true);
            var method = targetType.GetMethod("Read");
            var result = method.Invoke(target, new object[] { path });
            if (!(result is List<TEntity>))
            {
                throw new Exception("ExcelHelper返回结果类型有误");
            }
            return result as List<TEntity>;
        }

        private static ISheet LoadFile(string path)
        {
            //path = HttpContext.Current.Server.MapPath(path);
            if (!File.Exists(path))
            {
                throw new Exception("不存在此文件");
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
                throw new Exception("上传文件类型出错");
            }

            ISheet sheet = book.GetSheetAt(0);

            if (sheet.GetRow(1) == null)
            {
                throw new Exception("不存在数据");
            }

            return sheet;
        }

        public static List<TEntity> ReadFile(string path)
        {
            var baseType = typeof(TEntity);
            var attribute = baseType.GetCustomAttribute<ExcelSheetOptionAttribute>();
            int startX = attribute?.StartX ?? 0;
            int startY = attribute?.StartY ?? 1;
            var fields = baseType.GetProperties().ToList();
            var list = new List<TEntity>();
            var sheet = LoadFile(path);
            sheet.GetRow(0);
            Dictionary<int, PropertyInfo> fieldDic = new Dictionary<int, PropertyInfo>();
            foreach (var field in fields)
            {
                if (baseType.FullName != null)
                {
                    var cellOption = field.GetCustomAttribute<ExcelCellOptionAttribute>();
                    if (cellOption == null)
                    {
                        continue;
                    }

                    if (fieldDic.ContainsKey(cellOption.ColumnIndex))
                    {
                        throw new Exception($"配置多个列序号:{cellOption.ColumnIndex}");
                    }
                    fieldDic.Add(cellOption.ColumnIndex, field);
                }
            }
            // 开始读取数据
            if (baseType.FullName == null)
            {
                throw new Exception("目标下载数据类型错误");
            }

            for (int i = startY; ; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                {
                    break;
                }

                var obj = baseType.Assembly.CreateInstance(baseType.FullName);
                foreach (var info in fieldDic)
                {
                    var cell = row.GetCell(info.Key);
                    if (cell != null)
                    {
                        try
                        {
                            if (info.Value.PropertyType.FullName == typeof(DateTime).FullName || info.Value.PropertyType.FullName == typeof(DateTime?).FullName)
                            {
                                var cellValue = cell.StringCellValue;
                                info.Value.SetValue(obj, DateTime.Parse(cellValue));
                            }
                            else if (info.Value.PropertyType.FullName == typeof(string).FullName)
                            {
                                var cellValue = cell.StringCellValue;
                                info.Value.SetValue(obj, cellValue);
                            }
                            else if (info.Value.PropertyType.FullName == typeof(decimal).FullName || info.Value.PropertyType.FullName == typeof(decimal?).FullName)
                            {
                                var cellValue = (decimal)cell.NumericCellValue;
                                info.Value.SetValue(obj, cellValue);
                            }
                            else if (info.Value.PropertyType.FullName == typeof(double).FullName || info.Value.PropertyType.FullName == typeof(double?).FullName)
                            {
                                var cellValue = cell.NumericCellValue;
                                info.Value.SetValue(obj, cellValue);
                            }
                            else if (info.Value.PropertyType.FullName == typeof(int).FullName || info.Value.PropertyType.FullName == typeof(int?).FullName)
                            {
                                var cellValue = (int)cell.NumericCellValue;
                                info.Value.SetValue(obj, cellValue);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                if (obj is TEntity instance)
                {
                    list.Add(instance);
                }
            }
            return list;
        }

        public static List<TEntity> ReadFiles(string[] pathes)
        {
            var result = new List<TEntity>();
            foreach (var pathe in pathes)
            {
                result.AddRange(ReadFile(pathe));
            }
            return result;
        }

        public static FileStream ExportErrorFile(List<TEntity> entities, Dictionary<int, string> hasErrors)
        {
            return null;
        }
    }
}
