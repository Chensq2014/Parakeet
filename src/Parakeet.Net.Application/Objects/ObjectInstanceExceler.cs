//using Akso.Ioc;
//using Parakeet.Net.DataContracts.Dto;
//using Parakeet.Net.DataContracts.EnumMap;
//using Parakeet.Net.DataContracts.Models;
//using Parakeet.Net.Domain.IProvider;
//using Parakeet.Net.FileAgent;
//using Parakeet.Net.Infrastructure;
//using Parakeet.Net.ServiceContracts.Dto;
//using Parakeet.Net.ServiceContracts.Interfaces;
//using EFCore.BulkExtensions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using NPOI.HSSF.Util;
//using NPOI.SS.UserModel;
//using NPOI.SS.Util;
//using NPOI.XSSF.UserModel;
//using RestSharp.Extensions;
//using System.Data;
//using System.Diagnostics;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.Uow;

//namespace Parakeet.Net.Application
//{
//    /// <summary>
//    /// 对象实例Excel导入导出服务
//    /// </summary>
//    [Dependency(ServiceLifetime.Transient)]
//    public class ObjectInstanceExcelerService : BaseExceler<ObjectInstanceDataModel>, IObjectInstanceExcelerService
//    {
//        private readonly IObjectConfigPackageCacheService _objectConfigPackageCacheService;
//        private readonly IBasicObjectInstanceService _objectInstanceService;
//        private readonly IObjectPicklistCacheService _picklistCacheService;
//        private readonly IMetaDataService _metaDataService;
//        private readonly IOrganizationCacheService _organizationCacheService;
//        private readonly IObjectRepeatRuleService _objectRepeatRuleService;
//        private readonly IBasicObjectRelationInstanceService _objectRelationInstanceService;
//        private readonly IBehaviorEventService _behaviorEventService;
//        IUnitOfWorkManager _unitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();
//        /// <summary>
//        /// 对象实例导入任务
//        /// </summary>
//        IAksoRepository<ObjectInstanceImportTask> InstanceImportTaskRepository => LazyServiceProvider.LazyGetRequiredService<IAksoRepository<ObjectInstanceImportTask>>();
//        /// <summary>
//        /// 对象实例数据导入详情
//        /// </summary>
//        IAksoRepository<ObjectInstanceImportTaskRecord> InstanceImportTaskRecordRepository => LazyServiceProvider.LazyGetRequiredService<IAksoRepository<ObjectInstanceImportTaskRecord>>();
//        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 10);
//        /// <summary>
//        /// 已查询缓存
//        /// </summary>
//        private Dictionary<string, ImExportExcelReturnDto> ExistFieldResultDicts = new Dictionary<string, ImExportExcelReturnDto>();

//        public ObjectInstanceExcelerService(
//            IObjectConfigPackageCacheService objectConfigPackageCacheService,
//            IBasicObjectInstanceService objectInstanceService,
//            IObjectPicklistCacheService picklistCacheService,
//            IMetaDataService metaDataService,
//            IObjectRepeatRuleService objectRepeatRuleService,
//            IBasicObjectRelationInstanceService objectRelationInstanceService,
//            IOrganizationCacheService organizationCacheService,
//            IBehaviorEventService behaviorEventService)
//        {
//            _objectConfigPackageCacheService = objectConfigPackageCacheService;
//            _objectInstanceService = objectInstanceService;
//            _picklistCacheService = picklistCacheService;
//            _metaDataService = metaDataService;
//            _organizationCacheService = organizationCacheService;
//            _behaviorEventService = behaviorEventService;
//            _objectRelationInstanceService = objectRelationInstanceService;
//            _objectRepeatRuleService = objectRepeatRuleService;
//        }

//        /// <summary>
//        /// 验证释放能导入Excel数据
//        /// </summary>
//        /// <param name="input">对象Code与文件key</param>
//        /// <returns>返回导出文件key</returns>
//        public async Task<ImExportExcelReturnDto> ValidateImportAsync(InputKeyCodeDto input)
//        {
//            var objConfig = _objectConfigPackageCacheService.GetObjectByCode(input.Code);
//            var package = objConfig.ConfigPackage;
//            //var templates = package.GetFunctionTemplateByObjectCode(new InputCodeDto { Code = objConfig.Code });
//            //var template = templates.FirstOrDefault();

//            var result = new ImExportExcelReturnDto
//            {
//                FileKey = input.Key,
//                Message = $"【{objConfig?.Name?.Cn}】对象数据验证成功,允许导入!"
//            };

//            var departStr = ".";
//            var computeStr = "-";
//            var ignoreStr = "&";
//            IWorkbook book = null;
//            var hasError = false;
//            try
//            {
//                #region 验证下载及book 阻断性错误 直接返回

//                book = await LoadFile(input.Key);
//                var sheet1 = book?.GetSheetAt(1);
//                if (book?.GetSheetAt(0) == null || book.GetSheetAt(0)?.GetRow(0) == null || sheet1?.GetRow(0) == null)
//                {
//                    result.HasError = true;
//                    result.Message = $"无数据或格式不对,请检查文件,表单及首行不能为空";
//                    result.ErrorMessages.Add(result.Message);
//                    throw new Exception(result.Message);
//                }

//                var mainObjSheetFirstRowCellTxt = GetCellTextValue(sheet1?.GetRow(0)?.GetCell(0));
//                if (mainObjSheetFirstRowCellTxt?.Split(departStr).Last() != input.Code)
//                {
//                    result.HasError = true;
//                    result.Message = $"表单【{sheet1.SheetName}({mainObjSheetFirstRowCellTxt?.Split(departStr).Last()})】主对象Code与模板配置主对象【{objConfig.Name?.Cn}({input.Code})】不一致";
//                    result.ErrorMessages.Add(result.Message);
//                    await SetStyleAndComment(sheet1?.GetRow(0)?.GetCell(0), result.Message);
//                    throw new Exception(result.Message);
//                }
//                #endregion

//                #region 验证格式与数据，将错误写入book

//                foreach (var sheet in book)
//                {
//                    var isBreakOut = false;
//                    if (sheet.SheetName != "导入说明")
//                    {
//                        var firstRow = sheet.GetRow(0);
//                        var firstRowCellTxt = GetCellTextValue(firstRow?.GetCell(0));
//                        var objectCode = firstRowCellTxt?.Split(departStr)?.Last();
//                        var objectConfig = package.GetObjectByCode(objectCode);
//                        if (objectConfig is null)
//                        {
//                            hasError = true;//result.HasError = true;
//                            result.Message = $"表单【{sheet.SheetName}】:配置包中无编码为【{objectCode}】的对象";
//                            result.ErrorMessages.Add(result.Message);
//                            break;
//                        }
//                        var headerRow = sheet.GetRow(1);
//                        var companyColumnIndex = default(int?);
//                        var uniqueDic = new Dictionary<string, List<object>>();

//                        #region 验证首行

//                        foreach (var cell in firstRow.Cells)
//                        {
//                            if (cell.ColumnIndex > 2)
//                            {
//                                break;
//                            }
//                            var cellText = GetCellTextValue(cell);
//                            var cellError = false;
//                            switch (cell.ColumnIndex)
//                            {
//                                case 0:
//                                    if (cellText.Contains(departStr))
//                                    {
//                                        continue;
//                                    }
//                                    else if (objectCode != cellText)
//                                    {
//                                        cellError = true;//result.HasError = true;
//                                    }
//                                    break;
//                                case 1:
//                                    if (!cellText.Contains("关联字段"))
//                                    {
//                                        cellError = true;//result.HasError = true;
//                                    }

//                                    break;
//                                case 2:
//                                    if (!cellText.Contains("手动填写"))
//                                    {
//                                        cellError = true;//result.HasError = true;
//                                    }
//                                    break;
//                                default: break;
//                            }

//                            if (cellError)
//                            {
//                                hasError = true;
//                                result.Message =
//                                    $"表单【{sheet.SheetName}】第{cell.RowIndex + 1}行,第{cell.ColumnIndex + 1}列 对象【{objectConfig.Name?.Cn}】首行字段格式不正确";
//                                result.ErrorMessages.Add(result.Message);
//                                await SetStyleAndComment(cell, result.Message);
//                            }
//                        }

//                        #endregion

//                        #region 验证表头

//                        var vResult = await ValidateAsync(new ValidateExcelInputDto { Row = headerRow, Key = input.Code });
//                        hasError = vResult.HasError ? vResult.HasError : hasError;
//                        companyColumnIndex = vResult.Index;
//                        var excelUniqueFieldCodes = vResult.Data != null
//                            ? (List<string>)vResult.Data
//                            : new List<string>();

//                        var objUniqueFields = package.GetFieldsByObjectId(objectConfig.Id)?
//                            .Where(x => x.IsUnique)//&&x.IsIndexed
//                            .ToList();
//                        var objectUniqeFieldCodes = objUniqueFields?.Select(x => x.Code).Distinct().ToList();
//                        var missingFieldCodes = objectUniqeFieldCodes?.Except(excelUniqueFieldCodes).ToList();
//                        if (missingFieldCodes?.Any() == true)
//                        {
//                            hasError = true;
//                            result.Message = $"对象【{objectConfig.Name?.Cn}】无唯一字段【{string.Join(",", missingFieldCodes.Select(x => x))}】;";
//                            result.ErrorMessages.Add(result.Message);
//                        }

//                        if (vResult.Messages.Any())
//                        {
//                            result.Messages.AddRange(vResult.Messages);
//                        }
//                        if (vResult.ErrorMessages.Any())
//                        {
//                            hasError = true;
//                            result.ErrorMessages.AddRange(vResult.ErrorMessages);
//                        }

//                        #endregion

//                        #region 数据行验证

//                        for (var rowIndex = 2; rowIndex <= sheet.LastRowNum; rowIndex++)
//                        {
//                            var dataRow = sheet.GetRow(rowIndex);
//                            if (dataRow is null)
//                            {
//                                result.Message = $"表单【{sheet.SheetName}】第{rowIndex + 1}行为空，验证结束。总共验证{rowIndex - 2}行";
//                                result.Messages.Add(result.Message);
//                                //isBreakOut = true;
//                                break;
//                            }
//                            foreach (var dataRowCell in dataRow.Cells)
//                            {
//                                #region CellValue

//                                string cellValue;
//                                try
//                                {
//                                    cellValue = dataRowCell.CellType == CellType.Numeric
//                                        ? $"{GetCellTextValue(dataRowCell)}"
//                                        : dataRowCell.StringCellValue;
//                                }
//                                catch (Exception exp)
//                                {
//                                    hasError = true;
//                                    result.Message = $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 计算cellValue出错:{exp.Message}";
//                                    result.ErrorMessages.Add(result.Message);
//                                    await SetStyleAndComment(dataRowCell, result.Message);
//                                    continue;//break;
//                                }

//                                #endregion

//                                #region 前三列

//                                var dataRowError = false;
//                                if (dataRowCell.ColumnIndex <= 2)
//                                {
//                                    switch (dataRowCell.ColumnIndex)
//                                    {
//                                        case 0:
//                                            continue;
//                                        case 1:
//                                            if (cellValue.HasValue())
//                                            {
//                                                if (!cellValue.StartsWith($"{objConfig.Code}{computeStr}"))
//                                                {
//                                                    dataRowError = true;
//                                                }
//                                            }
//                                            else if (!firstRowCellTxt.Equals(objectCode))
//                                            {
//                                                if (GetCellTextValue(dataRow.Cells[2]).HasValue())
//                                                {
//                                                    dataRowError = true;
//                                                }
//                                                else
//                                                {
//                                                    result.Message = $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列无关联字段Id数据，验证结束。总共验证{dataRowCell.RowIndex - 2}行";
//                                                    result.Messages.Add(result.Message);
//                                                    await SetStyleAndComment(dataRowCell, result.Message);
//                                                    isBreakOut = true;
//                                                }
//                                            }
//                                            break;
//                                        default:
//                                            {
//                                                if (cellValue.HasValue())
//                                                {
//                                                    if (!cellValue.StartsWith($"{objectConfig.Code}{computeStr}"))
//                                                    {
//                                                        dataRowError = true;
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    result.Message = $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列无Id数据，验证结束。总共验证{dataRowCell.RowIndex - 2}行";
//                                                    result.Messages.Add(result.Message);
//                                                    isBreakOut = true;
//                                                }
//                                                break;
//                                            }
//                                    }

//                                    if (isBreakOut)
//                                    {
//                                        break;
//                                    }

//                                    if (dataRowError)
//                                    {
//                                        hasError = dataRowError;
//                                        result.Message =
//                                            $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 对象【{objectConfig.Name?.Cn}】字段格式不正确";
//                                        result.ErrorMessages.Add(result.Message);
//                                        await SetStyleAndComment(dataRowCell, result.Message);
//                                    }
//                                    continue;
//                                }

//                                #endregion

//                                #region 后续列

//                                if (dataRowCell.ColumnIndex >= headerRow.Cells.Count)
//                                {
//                                    break;
//                                }
//                                //var headerRowCell = headerRow.Cells[dataRowCell.ColumnIndex];
//                                var headerRowCellValue = GetCellTextValue(headerRow.Cells[dataRowCell.ColumnIndex]);//.StringCellValue;
//                                var fieldCode = headerRowCellValue.Split(computeStr).Last();
//                                var isRefObjField = fieldCode.Contains(ignoreStr);
//                                var fieldInfo = isRefObjField
//                                    ? GetFieldConfigByUnionCode(fieldCode, ignoreStr, package)
//                                    : package.GetObjectFieldByCode(objectConfig.Id, fieldCode);
//                                var isUnique = headerRowCellValue.StartsWith($"#");


//                                if (fieldInfo is null)
//                                {
//                                    dataRowError = true;
//                                    hasError = dataRowError;
//                                    result.Message = $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 对象【{objectConfig.Name?.Cn}】字段{fieldCode}不存在";
//                                    result.ErrorMessages.Add(result.Message);
//                                    await SetStyleAndComment(dataRowCell, result.Message);//把错误消息也标注在单元格里
//                                    continue;//break;
//                                }

//                                if (isRefObjField)
//                                {
//                                    if (isRefObjField && fieldInfo.IsIndexed && fieldInfo.IsUnique)
//                                    {
//                                        //当前主对象的引用字段，赋值为从数据库取出来的json对象
//                                        var referCodes = fieldCode?.Split(ignoreStr).ToList();
//                                        if (referCodes?.Count > 2)
//                                        {
//                                            var validate = await IsFieldExistAsync($"{referCodes[1]}", fieldInfo,
//                                                FormatCellValueString(cellValue));
//                                            if (!(validate.HasError && Guid.TryParse(validate.Data.ToString(), out Guid instanceId)))
//                                            {
//                                                dataRowError = true;
//                                                result.Message = $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 引用对象字段{fieldInfo.Code}={cellValue}值不存在，不能建立引用关系!";
//                                                result.Messages.Add(result.Message);
//                                                await SetStyleAndComment(dataRowCell, result.Message);
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    if (isUnique)
//                                    {
//                                        if (cellValue.HasValue())
//                                        {
//                                            var validate = await ValidateFieldAndGetCellValueObj(fieldInfo, dataRowCell, cellValue, companyColumnIndex);
//                                            if (validate.HasError)
//                                            {
//                                                dataRowError = validate.HasError;
//                                                result.Message = validate.Message;
//                                                result.ErrorMessages.AddRange(validate.ErrorMessages);
//                                                await SetStyleAndComment(dataRowCell, result.Message);
//                                            }
//                                            else
//                                            {
//                                                if (uniqueDic.ContainsKey(fieldInfo.Code))
//                                                {
//                                                    if (uniqueDic[fieldInfo.Code].Contains(validate.Data))
//                                                    {
//                                                        dataRowError = true;
//                                                        result.Message = $"该唯一性字段【{fieldInfo.Code}】存在重复值:{JsonConvert.SerializeObject(validate.Data)}";
//                                                        result.ErrorMessages.AddRange(validate.ErrorMessages);
//                                                        await SetStyleAndComment(dataRowCell, result.Message);
//                                                    }
//                                                    else
//                                                    {
//                                                        uniqueDic[fieldInfo.Code].Add(validate.Data);
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    uniqueDic.Add(fieldInfo.Code, new List<object> { validate.Data });
//                                                }
//                                            }
//                                        }
//                                        else
//                                        {
//                                            dataRowError = true;
//                                            result.Message =
//                                                $"第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 对象【{objectConfig.Name?.Cn}】字段{fieldInfo.Name?.Cn}有唯一检查，必填";
//                                            result.ErrorMessages.Add(result.Message);
//                                            await SetStyleAndComment(dataRowCell, result.Message);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (cellValue.HasValue())
//                                        {
//                                            var validate = await ValidateFieldAndGetCellValueObj(fieldInfo, dataRowCell, cellValue, companyColumnIndex);
//                                            if (validate.HasError)
//                                            {
//                                                dataRowError = validate.HasError;
//                                                result.Message = validate.Message;
//                                                result.ErrorMessages.AddRange(validate.ErrorMessages);
//                                                await SetStyleAndComment(dataRowCell, result.Message);
//                                            }
//                                        }
//                                    }

//                                }

//                                if (!hasError)
//                                {
//                                    hasError = dataRowError;
//                                }

//                                #endregion

//                            }

//                            if (isBreakOut)
//                            {
//                                break;
//                            }
//                        }

//                        #endregion

//                    }
//                }

//                #endregion

//                #region 验证完毕

//                if (hasError)
//                {
//                    throw new Exception($"{result.Message}");
//                }

//                #endregion
//            }
//            catch (Exception ex)
//            {
//                //将添加备注和边框提醒的book上传到文件服务器 返回文件key
//                result.HasError = true;
//                result.FileKey = book != null ? await SaveWorkbook(book) : result.FileKey;
//                result.FileName = $"验证失败_{objConfig.Name?.Cn}{DateTime.Now:yyyyMMddHHmmss}.xlsx";
//                result.Message = $"验证失败:{ex.Message}";
//                if (!result.ErrorMessages.Any())
//                {
//                    result.ErrorMessages.Add(result.Message);
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// 根据文件key与对象code导入数据已验证成功的.xlsx数据
//        /// </summary>
//        /// <param name="input">对象Code与文件key</param>
//        /// <returns>返回导出文件key</returns>
//        public override async Task<ImExportExcelReturnDto> ImportAsync(InputKeyCodeWithDataDto input)
//        {
//            var objConfig = _objectConfigPackageCacheService.GetObjectByCode(input.Code);
//            var package = objConfig.ConfigPackage;
//            var result = new ImExportExcelReturnDto
//            {
//                FileKey = input.Key,
//                Message = $"导入{objConfig?.Name?.Cn}对象及关联实例数据成功!\n"
//            };
//            var instanceGroups = new List<ObjectInstanceImportGroupDto>();

//            try
//            {
//                #region 循环读取每个sheet内容 组装各对象的实例集合

//                await SetInstanceGroup(input.Key, package, objConfig, result, instanceGroups);

//                #endregion

//                #region 统一保存数据到数据库 最后保存主对象

//                await SetAndSaveInstances(instanceGroups, package, input, result);

//                #endregion

//                result.SuccessCount = instanceGroups.Sum(x => x.SuccessCount);
//                result.ErrorCount = instanceGroups.Sum(x => x.ErrorCount);
//                if (result.SuccessCount == 0 && result.ErrorCount > 0)
//                {
//                    result.HasError = true;
//                    result.ErrorMessages.AddRange(result.Messages);
//                }
//                result.Message = $"总共{result.SuccessCount + result.ErrorCount}条实例数据,导入成功:{result.SuccessCount}条,失败或忽略:{result.ErrorCount}条,各对象实例导入详情\n:{result.Message}";
//                result.Messages.Add(result.Message);
//            }
//            catch (Exception ex)
//            {
//                result.HasError = true;
//                result.Message = $"{ex.Message}";
//                result.ErrorMessages.Add(result.Message);
//            }

//            return result;
//        }


//        /// <summary>
//        /// 创建对象导入任务
//        /// </summary>
//        /// <param name="input">对象Code与文件key</param>
//        /// <returns>返回导出文件key</returns>
//        public async Task<ImExportExcelReturnDto> CreateImportTaskAsync(InputKeyCodeWithDataDto input)
//        {
//            var objConfig = _objectConfigPackageCacheService.GetObjectByCode(input.Code);
//            var package = objConfig.ConfigPackage;
//            var result = new ImExportExcelReturnDto
//            {
//                FileKey = input.Key,
//                FileName = input.FileName,
//                Message = $"导入{objConfig?.Name?.Cn}对象及关联实例数据成功!\n"
//            };
//            var instanceGroups = new List<ObjectInstanceImportGroupDto>();

//            try
//            {
//                #region 循环读取每个sheet内容 组装各对象的实例集合

//                await PrepareInstanceGroupAsync(input.Key, package, objConfig, result, instanceGroups);

//                #endregion

//                #region 统一保存数据到数据库 最后保存主对象

//                var taskId = await CreateSaveInstanceTaskAsync(instanceGroups, package, input, result);

//                #endregion

//                result.SuccessCount = instanceGroups.Sum(x => x.SuccessCount);
//                result.ErrorCount = instanceGroups.Sum(x => x.ErrorCount);
//                if (result.SuccessCount == 0 && result.ErrorCount > 0)
//                {
//                    result.HasError = true;
//                    result.ErrorMessages.AddRange(result.Messages);
//                }
//                result.Data = taskId;
//                result.Message = $"导入任务创建完成";
//                result.Messages.Add(result.Message);
//            }
//            catch (Exception ex)
//            {
//                result.HasError = true;
//                result.Message = $"{ex.Message}";
//                result.ErrorMessages.Add(result.Message);
//            }

//            return result;
//        }


//        /// <summary>
//        /// 根据对象Code与按钮Id导出模板
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns>返回文件key</returns>
//        public override async Task<ImExportExcelReturnDto> ExportTemplateAsync(InputIdCodeDto input)
//        {
//            var objConfig = _objectConfigPackageCacheService.GetObjectByCode(input.Code);
//            var result = new ImExportExcelReturnDto
//            {
//                Message = $"导出【{objConfig?.Name?.Cn}】对象模板成功!"
//            };
//            if (objConfig == null)
//            {
//                result.HasError = true;
//                result.Message = $"未找到编码为【{input.Code}】的对象!";
//                result.ErrorMessages.Add(result.Message);
//                return await Task.FromResult(result);
//            }

//            var departStr = ".";
//            var computeStr = "-";
//            var package = objConfig.ConfigPackage;
//            var templates = package.GetFunctionTemplateByObjectCodeAndFunctionId(input);

//            if (templates.Any())
//            {
//                var template = templates.First();
//                if (template.Config != null)
//                {
//                    ResetFieldConfigs(template.Config);
//                    var fields = template.Config.Fields.OrderBy(x => x.ObjectOrder).ToList();
//                    //var uniqueFields = template.Config.ObjectUniqueFields;

//                    //组装excel sheet1
//                    IWorkbook book = new XSSFWorkbook();
//                    var sheet1 = book.CreateSheet($"导入说明");
//                    var rowIndex = 0;

//                    #region fonts

//                    var fontBase = book.CreateFont();
//                    //fontBase.FontHeightInPoints = 14;
//                    fontBase.Color = HSSFColor.Black.Index;

//                    var fontBold = book.CreateFont();
//                    //fontBold.CloneStyleFrom(fontBase);
//                    fontBold.IsBold = true;
//                    //fontBold.Color = HSSFColor.Black.Index;

//                    var fontBoldRedColor = book.CreateFont();
//                    //fontBoldRedColor.CloneStyleFrom(fontBold);
//                    fontBoldRedColor.IsBold = true;
//                    fontBoldRedColor.Color = HSSFColor.Red.Index;

//                    var fontUnique = book.CreateFont();
//                    fontUnique.Color = HSSFColor.Coral.Index;

//                    #endregion

//                    #region cellStyles

//                    var cellLeftStyle = book.CreateCellStyle();
//                    //自动换行
//                    cellLeftStyle.WrapText = true;
//                    //水平方向 左对齐
//                    cellLeftStyle.Alignment = HorizontalAlignment.Left;
//                    //竖直方向 垂直居中
//                    cellLeftStyle.VerticalAlignment = VerticalAlignment.Center;
//                    ////文本缩进
//                    //style.Indention = 2;
//                    cellLeftStyle.SetFont(fontBase);

//                    var cellCenterStyle = book.CreateCellStyle();
//                    //cellCenterStyle.CloneStyleFrom(cellStyle);
//                    cellCenterStyle.WrapText = true;
//                    cellCenterStyle.Alignment = HorizontalAlignment.Center;
//                    cellCenterStyle.SetFont(fontBase);

//                    var cellRedBoldStyle = book.CreateCellStyle();
//                    //redBoldStyle.CloneStyleFrom(cellStyle);
//                    cellRedBoldStyle.WrapText = true;
//                    cellRedBoldStyle.Alignment = HorizontalAlignment.Left;
//                    cellRedBoldStyle.VerticalAlignment = VerticalAlignment.Center;
//                    cellRedBoldStyle.SetFont(fontBoldRedColor);

//                    var headCenterStyle = book.CreateCellStyle();
//                    //headCenterStyle.CloneStyleFrom(cellStyle);
//                    headCenterStyle.WrapText = true;
//                    headCenterStyle.Alignment = HorizontalAlignment.Center;
//                    //headCenterStyle.FillForegroundColor = HSSFColor.Yellow.Index;
//                    //headCenterStyle.FillPattern = FillPattern.Squares;
//                    //headCenterStyle.FillBackgroundColor = HSSFColor.Yellow.Index;
//                    //headCenterStyle.BorderBottom = BorderStyle.Medium;
//                    //headCenterStyle.BottomBorderColor = HSSFColor.Blue.Index;
//                    headCenterStyle.SetFont(fontBold);

//                    var headLeftStyle = book.CreateCellStyle();
//                    headLeftStyle.WrapText = true;
//                    headLeftStyle.Alignment = HorizontalAlignment.Left;
//                    headLeftStyle.SetFont(fontBold);

//                    var headLeftWithBackGroundColorStyle = book.CreateCellStyle();
//                    headLeftWithBackGroundColorStyle.WrapText = true;
//                    headLeftWithBackGroundColorStyle.Alignment = HorizontalAlignment.Left;
//                    headLeftWithBackGroundColorStyle.FillForegroundColor = HSSFColor.SkyBlue.Index;
//                    headLeftWithBackGroundColorStyle.FillBackgroundColor = HSSFColor.SkyBlue.Index;
//                    headLeftWithBackGroundColorStyle.FillPattern = FillPattern.Squares;
//                    headLeftWithBackGroundColorStyle.SetFont(fontBold);

//                    var uniqueLeftStyle = book.CreateCellStyle();
//                    uniqueLeftStyle.WrapText = true;
//                    uniqueLeftStyle.Alignment = HorizontalAlignment.Left;
//                    uniqueLeftStyle.SetFont(fontUnique);

//                    var uniqueCenterStyle = book.CreateCellStyle();
//                    uniqueCenterStyle.WrapText = true;
//                    uniqueCenterStyle.Alignment = HorizontalAlignment.Center;
//                    uniqueCenterStyle.SetFont(fontUnique);

//                    //var dateFormat = book.CreateDataFormat();

//                    #endregion

//                    #region 第0行

//                    var row0 = sheet1.CreateRow(rowIndex++);
//                    row0.Height = 50 * 20;
//                    var cell0 = row0.CreateCell(0, CellType.String);
//                    var cell1 = row0.CreateCell(1, CellType.String);
//                    cell0.CellStyle = cellRedBoldStyle;
//                    cell1.CellStyle = cellRedBoldStyle;
//                    cell0.SetCellValue($"说明：\n操作前请按照字段填写说明准确填写导入模板中的基础数据表中\n需要手动填写的ID字段，请手动自己编号，导入时会根据ID导入关联关系。");
//                    cell1.SetCellValue("");

//                    //合并第0行的0-1列
//                    var region = new CellRangeAddress(0, 0, 0, 1);
//                    sheet1.AddMergedRegion(region);
//                    ////边框绘制
//                    //((HSSFSheet)sheet1).SetEnclosedBorderOfRegion(region, BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);

//                    #endregion

//                    #region 第1行

//                    var row1 = sheet1.CreateRow(rowIndex++);
//                    var row1Cell0 = row1.CreateCell(0, CellType.String);
//                    var row1Cell1 = row1.CreateCell(1, CellType.String);
//                    row1Cell0.CellStyle = headLeftWithBackGroundColorStyle;//headLeftStyle;
//                    row1Cell1.CellStyle = headLeftWithBackGroundColorStyle;//headLeftStyle;
//                    row1Cell0.SetCellValue("字段");
//                    row1Cell1.SetCellValue("填写说明");

//                    #endregion

//                    #region 字段按对象分组组装sheet对象模板

//                    var objectFieldGroup = fields.GroupBy(x => new
//                    {
//                        //ObjectCode = x.ObjectInfo.Code,
//                        ObjGroupOrder = x.ObjectOrder,
//                        ObjGroupCode = $"{(x.UnionCode.Contains(departStr) ? $"{x.UnionCode.Split(departStr).Last()}{departStr}" : string.Empty)}{x.ObjectInfo.Code}"
//                    })
//                    .OrderBy(x => x.Key.ObjGroupOrder)
//                    .ToList();

//                    foreach (var group in objectFieldGroup)
//                    {
//                        //统一局部变量
//                        var mainObjFieldCodeLink = group.Key.ObjGroupCode.Contains(departStr) ? $"{group.Key.ObjGroupCode.Split(departStr).First()}{departStr}" : "";

//                        #region objFieldDescriptionRow_sheet1

//                        var groupKey = group.First().ObjectInfo;
//                        var objRow = sheet1.CreateRow(rowIndex++);
//                        var objRowCell0 = objRow.CreateCell(0, CellType.String);
//                        var objRowCell1 = objRow.CreateCell(1, CellType.String);
//                        objRowCell0.CellStyle = cellLeftStyle;
//                        objRowCell1.CellStyle = cellLeftStyle;
//                        objRowCell0.SetCellValue($"对象:【{mainObjFieldCodeLink}{groupKey.Name}({groupKey.Code})】");
//                        objRowCell1.SetCellValue($"【{groupKey.Code}】");
//                        //合并当前行的0-1列
//                        sheet1.AddMergedRegion(new CellRangeAddress(objRow.RowNum, objRow.RowNum, 0, 1));

//                        #endregion

//                        #region objectSheet

//                        //创建对象表单
//                        var objectSheet = book.CreateSheet($"{mainObjFieldCodeLink}{RemoveSpecialChar(groupKey.Name)}");
//                        var objectSheetRowIndex = 0;
//                        var objectSheetRowCellIndex = 0;

//                        #region 第0行

//                        var firstRow = objectSheet.CreateRow(objectSheetRowIndex++);
//                        var firstRowCell0 = firstRow.CreateCell(0, CellType.String);
//                        var firstRowCell1 = firstRow.CreateCell(1, CellType.String);
//                        var firstRowCell2 = firstRow.CreateCell(2, CellType.String);
//                        firstRowCell0.CellStyle = cellLeftStyle;
//                        firstRowCell1.CellStyle = uniqueLeftStyle;
//                        firstRowCell2.CellStyle = uniqueCenterStyle;

//                        firstRowCell0.SetCellValue($"{mainObjFieldCodeLink}{groupKey.Code}");//说明
//                        firstRowCell1.SetCellValue($"关联字段");
//                        firstRowCell2.SetCellValue($"#手动填写");

//                        #endregion

//                        #region 第1行

//                        var headerRow = objectSheet.CreateRow(objectSheetRowIndex++);
//                        var headerRowCel0 = headerRow.CreateCell(objectSheetRowCellIndex++, CellType.String);
//                        var headerRowCel1 = headerRow.CreateCell(objectSheetRowCellIndex++, CellType.String);
//                        var headerRowCel2 = headerRow.CreateCell(objectSheetRowCellIndex++, CellType.String);
//                        headerRowCel0.CellStyle = headLeftStyle;
//                        headerRowCel1.CellStyle = headLeftStyle;
//                        headerRowCel2.CellStyle = headCenterStyle;
//                        headerRowCel0.SetCellValue($"字段");
//                        //寻找当前对象的主对象字段 使用departStr前缀分隔 如果为主对象自己则无前缀
//                        headerRowCel1.SetCellValue($"{mainObjFieldCodeLink}{objConfig.Name?.Cn}{computeStr}Id");
//                        headerRowCel2.SetCellValue($"#{groupKey.Name}{computeStr}Id");

//                        #endregion

//                        #region 示例行和列宽设置

//                        var range = Enumerable.Range(1, 10);
//                        foreach (var number in range)
//                        {
//                            var dataRow = objectSheet.CreateRow(objectSheetRowIndex++);
//                            var dataRowCel0 = dataRow.CreateCell(0, CellType.String);
//                            var dataRowCel1 = dataRow.CreateCell(1, CellType.String);
//                            var dataRowCel2 = dataRow.CreateCell(2, CellType.String);
//                            dataRowCel0.CellStyle = cellLeftStyle;
//                            dataRowCel1.CellStyle = cellLeftStyle;
//                            dataRowCel2.CellStyle = cellCenterStyle;
//                            dataRowCel0.SetCellValue($"");
//                            dataRowCel1.SetCellValue($"{objConfig.Code}{computeStr}{number}");
//                            dataRowCel2.SetCellValue($"{groupKey.Code}{computeStr}{number}");
//                        }

//                        objectSheet.DefaultColumnWidth = 40;
//                        objectSheet.SetColumnWidth(0, 30 * 256);
//                        #endregion

//                        #endregion

//                        #region objFieldRowCells objectSheet列信息

//                        foreach (var field in group)
//                        {
//                            //Log4Helper.Debug($"ObjectId:{field.ObjectInfo.Id}_FieldCode:{field.Code}");
//                            var fieldInfo = field.IsRelationshipField
//                                ? package.GetObjectFieldByCode(field.OrginObjectInfo.Id, field.Code)
//                                : package.GetObjectFieldByCode(field.ObjectInfo.Id, field.Code);

//                            #region sheet1

//                            {
//                                //每一列的名称，说明内容
//                                var sheetRow = sheet1.CreateRow(rowIndex++);
//                                var sheetRowCell0 = sheetRow.CreateCell(0, CellType.String);
//                                var sheetRowCell1 = sheetRow.CreateCell(1, CellType.String);
//                                sheetRowCell0.CellStyle = cellLeftStyle;
//                                sheetRowCell1.CellStyle = cellLeftStyle;
//                                sheetRowCell0.SetCellValue($"{field.Name}");
//                                sheetRowCell1.SetCellValue(await GetFieldHelpInputRemark(fieldInfo));
//                                //if (fieldInfo.DataType == EnumObjectFieldDataType.日期)
//                                //{
//                                //    sheetRowCell1.CellStyle.DataFormat = dateFormat.GetFormat("###,###,###,###,###,##0.000000");
//                                //}
//                            }

//                            #endregion

//                            #region objectSheet

//                            {
//                                var firstRowCell = firstRow.CreateCell(objectSheetRowCellIndex, CellType.String);
//                                var headerRowcCell = headerRow.CreateCell(objectSheetRowCellIndex, CellType.String);

//                                if (field.IsUnique)
//                                {
//                                    firstRowCell.SetCellValue($"#唯一性检查");
//                                    headerRowcCell.SetCellValue($"#{field.Name}{computeStr}{(field.IsRelationshipField ? field.UnionCode : field.Code)}");
//                                    firstRowCell.CellStyle = uniqueCenterStyle;
//                                    headerRowcCell.CellStyle = headCenterStyle;
//                                }
//                                else
//                                {

//                                    //firstRowCell.SetCellValue($"");
//                                    headerRowcCell.SetCellValue($"{field.Name}{computeStr}{(field.IsRelationshipField ? field.UnionCode : field.Code)}");
//                                    firstRowCell.CellStyle = cellCenterStyle;
//                                    headerRowcCell.CellStyle = headCenterStyle;
//                                }

//                                foreach (var num in range)
//                                {
//                                    var dataRow = objectSheet.GetRow(headerRow.RowNum + num);
//                                    var dataRowCell = dataRow.CreateCell(objectSheetRowCellIndex, CellType.String);
//                                    dataRowCell.CellStyle = cellCenterStyle;
//                                    dataRowCell.CellStyle = cellCenterStyle;
//                                    //dataRowCell.SetCellValue($"");
//                                }

//                                objectSheetRowCellIndex++;
//                            }

//                            #endregion

//                        }

//                        #endregion

//                        #region wrapRow

//                        var wrapRow = sheet1.CreateRow(rowIndex++);
//                        wrapRow.CreateCell(0, CellType.String);
//                        wrapRow.CreateCell(0, CellType.String);
//                        //合并当前行的0-1列
//                        sheet1.AddMergedRegion(new CellRangeAddress(wrapRow.RowNum, wrapRow.RowNum, 0, 1));

//                        #endregion

//                    }

//                    #endregion

//                    #region 特殊列列宽

//                    sheet1.DefaultColumnWidth = 30;
//                    sheet1.SetColumnWidth(0, 30 * 256);
//                    sheet1.SetColumnWidth(1, 100 * 256);

//                    #endregion

//                    #region 将book上传到文件服务器 返回文件key

//                    result.FileKey = await SaveWorkbook(book);
//                    result.FileName = $"{RemoveSpecialChar(objConfig.Name?.Cn)}模板{DateTime.Now:yyyyMMddHHmmss}.xlsx";
//                    #endregion
//                }
//            }
//            else
//            {
//                result.HasError = true;
//                result.Message = $"未找到对象【{input.Code}】_功能按钮【{input.Id}】的导出模板,请检查参数或配置包!";
//                result.ErrorMessages.Add(result.Message);
//            }

//            return await Task.FromResult(result);
//        }

//        /// <summary>
//        /// 导出数据
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        protected override async Task<ImExportExcelReturnDto> ExportAsync(IQueryable<ObjectInstanceDataModel> data)
//        {
//            var result = new ImExportExcelReturnDto();
//            return await Task.FromResult(result);
//        }

//        /// <summary>
//        /// 验证行
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public override async Task<ImExportExcelReturnDto> ValidateAsync(ValidateExcelInputDto input)
//        {
//            var departStr = ".";
//            var computeStr = "-";
//            var ignoreStr = "&";
//            var sheet = input.Row.Sheet;
//            var objConfig = _objectConfigPackageCacheService.GetObjectByCode(input.Key);
//            var objectCode = GetCellTextValue(sheet.GetRow(0).GetCell(0))?.Split(departStr).Last();
//            var objectConfig = _objectConfigPackageCacheService.GetObjectByCode(objectCode);
//            var package = objConfig.ConfigPackage;
//            var result = new ImExportExcelReturnDto();
//            var uniqueFieldCodes = new List<string>();
//            foreach (var headerRowCell in input.Row.Cells)
//            {
//                #region 前三列

//                var headerRowCellStr = GetCellTextValue(headerRowCell);
//                if (headerRowCell.ColumnIndex <= 2)
//                {
//                    if (headerRowCellStr.Equals("字段") || headerRowCellStr.EndsWith($"{computeStr}Id"))
//                    {
//                        if (headerRowCell.ColumnIndex == 1)
//                        {
//                            if (headerRowCellStr.Contains(departStr))
//                            {
//                                var mainObjFieldCode = headerRowCellStr.Split(departStr).First();
//                                var mainObjFieldInfo = package.GetObjectFieldByCode(objConfig.Id, mainObjFieldCode);
//                                if (mainObjFieldInfo == null)
//                                {
//                                    result.HasError = true;
//                                    result.Message = $"表单【{sheet.SheetName}】第{headerRowCell.RowIndex + 1}行,第{headerRowCell.ColumnIndex + 1}列 表头关联字段 对象【{objectConfig.Name?.Cn}】在主对象【{objConfig.Name?.Cn}】上的字段编码【{mainObjFieldCode}】未找到.";
//                                    result.ErrorMessages.Add(result.Message);
//                                    await SetStyleAndComment(headerRowCell, result.Message);
//                                }
//                            }
//                        }
//                        continue;
//                    }
//                    result.HasError = true;
//                    result.Message = $"表单【{sheet.SheetName}】第{headerRowCell.RowIndex + 1}行,第{headerRowCell.ColumnIndex + 1}列 对象【{objectConfig.Name?.Cn}】表头字段格式不正确";
//                    result.ErrorMessages.Add(result.Message);
//                    await SetStyleAndComment(headerRowCell, result.Message);
//                }

//                #endregion

//                #region 后续列
//                var fieldCode = headerRowCellStr.Split(computeStr).Last();
//                if (fieldCode.HasValue())
//                {
//                    var isRefObjField = fieldCode.Contains(ignoreStr);
//                    var fieldInfo = isRefObjField
//                        ? GetFieldConfigByUnionCode(fieldCode, ignoreStr, package)
//                        : package.GetObjectFieldByCode(objectConfig.Id, fieldCode);

//                    if (fieldInfo == null)
//                    {
//                        result.HasError = true;
//                        result.Message = $"表单【{sheet.SheetName}】第{headerRowCell.RowIndex + 1}行,第{headerRowCell.ColumnIndex + 1}列 对象【{objectConfig.Name?.Cn}】表头字段编码【{fieldCode}】不存在";
//                        result.ErrorMessages.Add(result.Message);
//                        await SetStyleAndComment(headerRowCell, result.Message);
//                    }
//                    else
//                    {
//                        //var isUnique = fieldInfo.IsUnique;//headerRowCell.StringCellValue.StartsWith($"#");
//                        if (fieldInfo.IsUnique && !isRefObjField)
//                        {
//                            uniqueFieldCodes.Add(fieldCode);
//                        }
//                        //判断是否有公司，根据获取公司Id
//                        if (fieldInfo.DataType == EnumObjectFieldDataType.公司列表)
//                        {
//                            result.Index = headerRowCell.ColumnIndex;
//                        }
//                    }
//                }
//                else
//                {
//                    result.HasError = true;
//                    result.Message = $"表单【{sheet.SheetName}】第{headerRowCell.RowIndex + 1}行,第{headerRowCell.ColumnIndex + 1}列 对象【{objectConfig.Name?.Cn}】表头信息无字段编码";
//                    result.ErrorMessages.Add(result.Message);
//                    await SetStyleAndComment(headerRowCell, result.Message);
//                }

//                #endregion
//            }
//            //uniqueFieldCodes 是否需要加$"{computeStr}Id" 与$"引用字段Code{computeStr}Id" 
//            result.Data = uniqueFieldCodes;//.Distinct().ToList()
//            return result;
//        }


//        #region 私有函数

//        /// <summary>
//        /// 计算CompanyColumnIndex
//        /// </summary>
//        /// <param name="headerRow"></param>
//        /// <param name="package"></param>
//        /// <param name="objectId"></param>
//        /// <returns></returns>
//        private int? GetCompanyColumnIndex(IRow headerRow, IConfigPackage package, Guid objectId, string computeStr)
//        {
//            var companyColumnIndex = default(int?);
//            foreach (var headerRowCell in headerRow.Cells)
//            {
//                if (headerRowCell.ColumnIndex > 2)
//                {
//                    var fieldCode = GetCellTextValue(headerRowCell).Split(computeStr).Last();
//                    var isRefObjField = fieldCode.Contains("&");
//                    if (fieldCode.HasValue() && !isRefObjField)
//                    {
//                        var fieldInfo = package.GetObjectFieldByCode(objectId, fieldCode);
//                        //判断是否有公司，根据获取公司Id
//                        if (fieldInfo?.DataType == EnumObjectFieldDataType.公司列表)
//                        {
//                            companyColumnIndex = headerRowCell.ColumnIndex;
//                            break;
//                        }
//                    }
//                }
//            }

//            return companyColumnIndex;
//        }

//        /// <summary>
//        /// 设置对象实例组数据
//        /// </summary>
//        /// <param name="fileKey"></param>
//        /// <param name="package"></param>
//        /// <param name="objConfig"></param>
//        /// <param name="result"></param>
//        /// <param name="instanceGroups"></param>
//        /// <returns></returns>
//        private async Task SetInstanceGroup(string fileKey, IConfigPackage package, BasicObjectConfig objConfig,
//            ImExportExcelReturnDto result, List<ObjectInstanceImportGroupDto> instanceGroups)
//        {
//            var departStr = ".";
//            var computeStr = "-";
//            var ignoreStr = "&";
//            var book = await LoadFile(fileKey);
//            var objGroupOrder = 1;
//            foreach (var sheet in book)
//            {
//                var companyColumnIndex = default(int?);
//                if (sheet.SheetName != "导入说明")
//                {
//                    #region 对象配置参数

//                    var firstRow = sheet.GetRow(0);
//                    var firstRowCellTxt = GetCellTextValue(firstRow?.GetCell(0));
//                    var objectCode = firstRowCellTxt?.Split(departStr).Last();
//                    var objectConfig = _objectConfigPackageCacheService.GetObjectByCode(objectCode);
//                    var headerRow = sheet.GetRow(1);

//                    #endregion

//                    #region 关联字段 当前对象在主对象上的字段信息

//                    //【当前对象】在主对象上的字段信息
//                    var headerRowLinkFieldCell = headerRow.GetCell(1);
//                    var hasLinkField = GetCellTextValue(headerRowLinkFieldCell).Contains(departStr);
//                    var mainObjFieldCode =
//                        hasLinkField ? GetCellTextValue(headerRowLinkFieldCell).Split(departStr).First() : string.Empty;
//                    var mainObjFieldInfo = hasLinkField
//                        ? package.GetObjectFieldByCode(objConfig.Id, mainObjFieldCode)
//                        : null;
//                    if (hasLinkField && mainObjFieldInfo == null)
//                    {
//                        result.HasError = true;
//                        result.Message =
//                            $"表单【{sheet.SheetName}】第{headerRowLinkFieldCell.RowIndex + 1}行,第{headerRowLinkFieldCell.ColumnIndex + 1}列 关联字段【{mainObjFieldCode}】不存在，忽略此对象所有实例导入";
//                        result.ErrorMessages.Add(result.Message);
//                        continue;
//                    }

//                    #endregion

//                    #region 表头列找 companyColumnIndex

//                    companyColumnIndex = GetCompanyColumnIndex(headerRow, package, objectConfig.Id, computeStr);

//                    #endregion

//                    #region 数据行

//                    var objectInstanceGroup = new ObjectInstanceImportGroupDto
//                    {
//                        ObjGroupOrder = objGroupOrder++,
//                        MainObjFieldInfo = mainObjFieldInfo,
//                        ObjectInfo = new ObjectInfoConfig
//                        {
//                            Id = objectConfig.Id,
//                            Name = objectConfig.Name?.Cn,
//                            Code = objectConfig.Code
//                        }
//                    };
//                    instanceGroups.Add(objectInstanceGroup);

//                    await SetObjectInstancesBySheetRows(sheet, objectConfig, result, headerRow, package, companyColumnIndex,
//                        objectInstanceGroup, instanceGroups);

//                    #endregion
//                }
//            }

//            //如果为主对象自己 则更新自己的Parent 排除循环引用  这个应该最后执行
//            var mainObjInstanceGroup = instanceGroups.FirstOrDefault(x => x.ObjGroupOrder == 1);
//            if (mainObjInstanceGroup?.ObjectInstanceImportDtos.Any() == true)
//            {
//                foreach (var objectInstanceImportDto in mainObjInstanceGroup.ObjectInstanceImportDtos)
//                {
//                    if (objectInstanceImportDto.CustomReferenceObjectId.HasValue() && objectInstanceImportDto.CustomReferenceObjectId != objectInstanceImportDto.CustomId)
//                    {
//                        var parent = mainObjInstanceGroup.ObjectInstanceImportDtos
//                            .FirstOrDefault(x => x.Id != objectInstanceImportDto.Id && x.CustomId == objectInstanceImportDto.CustomReferenceObjectId);
//                        if (parent != null)
//                        {
//                            if (objectInstanceImportDto.JsonJObject.ContainsKey($"Parent"))
//                            {
//                                objectInstanceImportDto.JsonJObject[$"Parent"] = JToken.FromObject(parent.JsonJObject);
//                            }
//                            else
//                            {
//                                objectInstanceImportDto.JsonJObject.Add(new JProperty($"Parent", JToken.FromObject(parent.JsonJObject)));
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 设置对象实例组数据用于实例保存
//        /// </summary>
//        /// <param name="fileKey"></param>
//        /// <param name="package"></param>
//        /// <param name="objConfig"></param>
//        /// <param name="result"></param>
//        /// <param name="instanceGroups"></param>
//        /// <returns></returns>
//        private async Task PrepareInstanceGroupAsync(string fileKey, IConfigPackage package, BasicObjectConfig objConfig,
//            ImExportExcelReturnDto result, List<ObjectInstanceImportGroupDto> instanceGroups)
//        {
//            var departStr = ".";
//            var computeStr = "-";
//            var ignoreStr = "&";
//            var book = await LoadFile(fileKey);
//            var objGroupOrder = 1;
//            foreach (var sheet in book)
//            {
//                var companyColumnIndex = default(int?);
//                if (sheet.SheetName != "导入说明")
//                {
//                    #region 对象配置参数

//                    var firstRow = sheet.GetRow(0);
//                    var firstRowCellTxt = GetCellTextValue(firstRow?.GetCell(0));
//                    var objectCode = firstRowCellTxt?.Split(departStr).Last();
//                    var objectConfig = _objectConfigPackageCacheService.GetObjectByCode(objectCode);
//                    var headerRow = sheet.GetRow(1);

//                    #endregion

//                    #region 关联字段 当前对象在主对象上的字段信息

//                    //【当前对象】在主对象上的字段信息
//                    var headerRowLinkFieldCell = headerRow.GetCell(1);
//                    var hasLinkField = GetCellTextValue(headerRowLinkFieldCell).Contains(departStr);
//                    var mainObjFieldCode =
//                        hasLinkField ? GetCellTextValue(headerRowLinkFieldCell).Split(departStr).First() : string.Empty;
//                    var mainObjFieldInfo = hasLinkField
//                        ? package.GetObjectFieldByCode(objConfig.Id, mainObjFieldCode)
//                        : null;
//                    if (hasLinkField && mainObjFieldInfo == null)
//                    {
//                        result.HasError = true;
//                        result.Message =
//                            $"表单【{sheet.SheetName}】第{headerRowLinkFieldCell.RowIndex + 1}行,第{headerRowLinkFieldCell.ColumnIndex + 1}列 关联字段【{mainObjFieldCode}】不存在，忽略此对象所有实例导入";
//                        result.ErrorMessages.Add(result.Message);
//                        continue;
//                    }

//                    #endregion

//                    #region 表头列找 companyColumnIndex

//                    companyColumnIndex = GetCompanyColumnIndex(headerRow, package, objectConfig.Id, computeStr);

//                    #endregion

//                    #region 数据行

//                    var objectInstanceGroup = new ObjectInstanceImportGroupDto
//                    {
//                        ObjGroupOrder = objGroupOrder++,
//                        MainObjFieldInfo = mainObjFieldInfo,
//                        ObjectInfo = new ObjectInfoConfig
//                        {
//                            Id = objectConfig.Id,
//                            Name = objectConfig.Name?.Cn,
//                            Code = objectConfig.Code
//                        }
//                    };
//                    instanceGroups.Add(objectInstanceGroup);

//                    await PrepareObjectInstancesBySheetRowsAsync(sheet, objectConfig, result, headerRow, package, companyColumnIndex, objectInstanceGroup, instanceGroups);

//                    #endregion
//                }
//            }

//            //如果为主对象自己 则更新自己的Parent 排除循环引用  这个应该最后执行
//            var mainObjInstanceGroup = instanceGroups.FirstOrDefault(x => x.ObjGroupOrder == 1);
//            if (mainObjInstanceGroup?.ObjectInstanceImportDtos.Any() == true)
//            {
//                foreach (var objectInstanceImportDto in mainObjInstanceGroup.ObjectInstanceImportDtos)
//                {
//                    if (objectInstanceImportDto.CustomReferenceObjectId.HasValue() && objectInstanceImportDto.CustomReferenceObjectId != objectInstanceImportDto.CustomId)
//                    {
//                        var parent = mainObjInstanceGroup.ObjectInstanceImportDtos
//                            .FirstOrDefault(x => x.Id != objectInstanceImportDto.Id && x.CustomId == objectInstanceImportDto.CustomReferenceObjectId);
//                        if (parent != null)
//                        {
//                            if (objectInstanceImportDto.JsonJObject.ContainsKey($"Parent"))
//                            {
//                                objectInstanceImportDto.JsonJObject[$"Parent"] = JToken.FromObject(parent.JsonJObject);
//                            }
//                            else
//                            {
//                                objectInstanceImportDto.JsonJObject.Add(new JProperty($"Parent", JToken.FromObject(parent.JsonJObject)));
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 对象实例数据行
//        /// </summary>
//        /// <param name="sheet"></param>
//        /// <param name="objectConfig"></param>
//        /// <param name="result"></param>
//        /// <param name="headerRow"></param>
//        /// <param name="package"></param>
//        /// <param name="companyColumnIndex"></param>
//        /// <param name="objectInstanceGroup"></param>
//        /// <param name="instanceGroups"></param>
//        private async Task SetObjectInstancesBySheetRows(ISheet sheet, BasicObjectConfig objectConfig, ImExportExcelReturnDto result,
//            IRow headerRow, IConfigPackage package, int? companyColumnIndex, ObjectInstanceImportGroupDto objectInstanceGroup, List<ObjectInstanceImportGroupDto> instanceGroups)
//        {
//            var computeStr = "-";
//            var ignoreStr = "&";
//            var isBreakOut = false;
//            for (var rowIndex = 2; rowIndex <= sheet.LastRowNum; rowIndex++)
//            {
//                var instanceId = SeqGuidHelper.NewGuid();
//                var instanceImportDto = new ObjectInstanceImportDto
//                {
//                    Id = instanceId,
//                    ObjectId = objectConfig.Id,
//                    ObjectName = objectConfig.Name,
//                    ObjectCode = objectConfig.Code,
//                    ConfigPackageId = objectConfig.ConfigPackage.Id,
//                    //构造一个 jsonObject
//                    JsonJObject = new JObject
//                    {
//                        { $"Id", JToken.FromObject(instanceId) },
//                        { $"ObjectId", JToken.FromObject(objectConfig.Id) },
//                        { $"ObjectCode", JToken.FromObject(objectConfig.Code) },
//                        { $"ObjectName", JToken.FromObject(objectConfig.Name) },
//                        { $"ConfigPackageId", JToken.FromObject(objectConfig.ConfigPackage.Id) }
//                    }
//                };
//                //var properties = instanceImportDto.GetType().GetProperties();
//                var dataRow = sheet.GetRow(rowIndex);
//                if (dataRow is null)
//                {
//                    result.Message = $"表单【{sheet.SheetName}】第{rowIndex + 1}行为空，导入结束。总共导入{rowIndex - 2}行";
//                    result.Messages.Add(result.Message);
//                    //isBreakOut = true;
//                    break;
//                }
//                foreach (var dataRowCell in dataRow.Cells)
//                {
//                    #region cellValue

//                    var cellValue = string.Empty;
//                    try
//                    {
//                        cellValue = dataRowCell.CellType == CellType.Numeric
//                            ? $"{GetCellTextValue(dataRowCell)}"
//                            : dataRowCell.StringCellValue;
//                    }
//                    catch (Exception exp)
//                    {
//                        result.HasError = true;
//                        result.Message =
//                            $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 忽略导入计算cellValue出错:{exp.Message}";
//                        result.ErrorMessages.Add(result.Message);
//                        //break;
//                    }

//                    #endregion

//                    #region 前三列 跳出条件

//                    if (dataRowCell.ColumnIndex <= 2)
//                    {
//                        switch (dataRowCell.ColumnIndex)
//                        {
//                            case 0:
//                                continue;
//                            case 1:
//                                if (cellValue.HasValue())
//                                {
//                                    instanceImportDto.CustomReferenceObjectId = cellValue;
//                                }
//                                else if (objectInstanceGroup.MainObjFieldInfo != null)
//                                {
//                                    result.Message =
//                                        $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列无关联字段Id数据，导入结束;总共导入{dataRowCell.RowIndex - 2}行";
//                                    result.Messages.Add(result.Message);
//                                    isBreakOut = true;
//                                }
//                                break;
//                            default:
//                                {
//                                    if (!cellValue.HasValue())
//                                    {
//                                        result.Message =
//                                            $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列无Id数据，导入结束;总共导入{dataRowCell.RowIndex - 2}行";
//                                        result.Messages.Add(result.Message);
//                                        isBreakOut = true;
//                                    }
//                                    else
//                                    {
//                                        instanceImportDto.CustomId = cellValue;
//                                    }

//                                    break;
//                                }
//                        }

//                        if (isBreakOut)
//                        {
//                            break;
//                        }

//                        continue;
//                    }

//                    #endregion

//                    #region 后续列
//                    if (dataRowCell.ColumnIndex >= headerRow.Cells.Count)
//                    {
//                        break;
//                    }
//                    var headerRowCellValue = GetCellTextValue(headerRow.Cells[dataRowCell.ColumnIndex]);//.StringCellValue;
//                    var fieldCode = headerRowCellValue.Split(computeStr).Last();
//                    var isRefObjField = fieldCode.Contains(ignoreStr);
//                    var fieldInfo = isRefObjField
//                        ? GetFieldConfigByUnionCode(fieldCode, ignoreStr, package)
//                        : package.GetObjectFieldByCode(objectConfig.Id, fieldCode);
//                    var isUnique = headerRowCellValue.StartsWith($"#");

//                    try
//                    {
//                        if (cellValue.HasValue())
//                        {
//                            if (isRefObjField && fieldInfo.IsIndexed)
//                            {
//                                //当前主对象的引用字段，赋值为从数据库取出来的json对象
//                                var referCodes = fieldCode?.Split(ignoreStr).ToList();
//                                if (referCodes?.Count > 2)
//                                {
//                                    var mainObjFieldCode = referCodes.Last();
//                                    var mainObjFieldConfig = package.GetObjectFieldByCode(objectConfig.Id, mainObjFieldCode);
//                                    var validate = await IsFieldExistAsync($"{referCodes[1]}", fieldInfo,
//                                        FormatCellValueString(cellValue));
//                                    if (validate.HasError)
//                                    {
//                                        var referObjInstance = await _objectInstanceService.GetObjectInstanceAsync(
//                                            new GetObjectInstanceDto
//                                            {
//                                                InstanceId = validate.Data.ToGuid(),
//                                                IsReadReferenceObject = false,
//                                                IsReadReferencedObject = false
//                                            });
//                                        //referObjInstance.Changed = false;
//                                        if (referObjInstance?.InstanceObject != null)
//                                        {
//                                            if (instanceImportDto.JsonJObject.ContainsKey($"{mainObjFieldCode}"))
//                                            {
//                                                //instanceImportDto.JsonJObject[$"{mainObjFieldCode}"] =
//                                                //    JToken.FromObject(mainObjFieldConfig.IsArray == true
//                                                //        ? new List<object> { referObjInstance.GetValues(1) }
//                                                //        : referObjInstance.GetValues(1));
//                                                if ((mainObjFieldConfig.IsArray == true) && (mainObjFieldConfig.DataType != EnumObjectFieldDataType.父对象))
//                                                {
//                                                    var refFieldJsonObj = instanceImportDto.JsonJObject[$"{mainObjFieldCode}"];
//                                                    if (refFieldJsonObj != null)
//                                                    {
//                                                        var refFieldArray = JArray.FromObject(refFieldJsonObj);
//                                                        refFieldArray?.Add(JToken.FromObject(referObjInstance.GetValues(1)));
//                                                        instanceImportDto.JsonJObject[$"{mainObjFieldCode}"] = JToken.FromObject(refFieldArray);
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    instanceImportDto.JsonJObject[$"{mainObjFieldCode}"] = JToken.FromObject(referObjInstance.GetValues(1));
//                                                }
//                                            }
//                                            else
//                                            {
//                                                instanceImportDto.JsonJObject.Add($"{mainObjFieldCode}",
//                                                    JToken.FromObject(mainObjFieldConfig.IsArray == true
//                                                        ? new List<object> { referObjInstance.GetValues(1) }
//                                                        : referObjInstance.GetValues(1)));
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        result.Message = $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 引用对象字段{fieldInfo.Code}={cellValue}值不存在，忽略此引用关系!";
//                                        result.Messages.Add(result.Message);
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                if (isUnique && fieldInfo.IsUnique)
//                                {
//                                    // 确保唯一字段有索引 才可以查询
//                                    if (fieldInfo.IsIndexed)
//                                    {
//                                        var validate = await IsFieldExistAsync($"{objectConfig.Code}", fieldInfo,
//                                            FormatCellValueString(cellValue));
//                                        if (validate.HasError)
//                                        {
//                                            instanceId = validate.Data.ToGuid();
//                                            instanceImportDto.Id = instanceId;
//                                            instanceImportDto.JsonJObject["Id"] = instanceImportDto.Id;
//                                            result.Messages.Add(validate.Message);
//                                        }
//                                    }
//                                }

//                                var cellReturnResult = await ValidateFieldAndGetCellValueObj(fieldInfo, dataRowCell,
//                                    cellValue, companyColumnIndex);
//                                if (cellReturnResult.HasError)
//                                {
//                                    //result.HasError = true;//导入环节-转换失败不影响总体导入错误
//                                    if (cellReturnResult.ErrorMessages.Any())
//                                    {
//                                        //result.ErrorMessages.AddRange(cellReturnResult.ErrorMessages);
//                                        result.Messages.AddRange(cellReturnResult.ErrorMessages);
//                                    }
//                                }

//                                var cellObj = cellReturnResult.Data;
//                                instanceImportDto.JsonJObject.Add($"{fieldInfo.Code}",
//                                    JToken.FromObject(cellObj)); //JsonConvert.SerializeObject(cellObj)
//                                //var property = properties.FirstOrDefault(p => p.Name == fieldInfo.Code);
//                                //if (property != null)
//                                //{
//                                //    property.SetValue(instanceImportDto, cellObj);
//                                //}

//                                //哪些特殊属性需要特殊设置的？ CompanyId PromoterId ResponsibleId Create
//                                instanceImportDto.CompanyId ??= fieldInfo.DataType == EnumObjectFieldDataType.公司列表
//                                    ? (cellObj as List<CompanyFieldValue>)?.FirstOrDefault()?.Id
//                                    : CurrentUser?.LoginCompanyId;
//                                instanceImportDto.PromoterId ??= fieldInfo.DataType == EnumObjectFieldDataType.用户列表 &&
//                                                                 fieldInfo.Code.StartsWith("Promoter")
//                                    ? (cellObj as List<UserFieldValue>)?.FirstOrDefault()?.Id
//                                    : CurrentUser?.UserId;
//                                instanceImportDto.ResponsibleId ??=
//                                    fieldInfo.DataType == EnumObjectFieldDataType.用户列表 &&
//                                    fieldInfo.Code.StartsWith("Responsible")
//                                        ? (cellObj as List<UserFieldValue>)?.FirstOrDefault()?.Id.ToString()
//                                        : CurrentUser?.UserId.ToString();

//                            }
//                        }
//                        else
//                        {
//                            ////property.SetValue(instanceImportDto, null);
//                            //instanceImportDto.JsonJObject.Add($"{fieldCode}",null);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        result.HasError = true;
//                        result.Message =
//                            $"表单【{sheet.SheetName}】第{dataRowCell.RowIndex + 1}行,第{dataRowCell.ColumnIndex + 1}列 计算cellValueObj出错:{ex.Message}";
//                        result.ErrorMessages.Add(result.Message);
//                        return;
//                    }

//                    #endregion
//                }

//                if (isBreakOut)
//                {
//                    break;
//                }

//                if (instanceImportDto.CreatorId == Guid.Empty)
//                {
//                    instanceImportDto.CreatorId = CurrentUser?.UserId ?? Guid.Empty;
//                    instanceImportDto.CreatedTime = DateTime.Now;
//                }

//                objectInstanceGroup.ObjectInstanceImportDtos.Add(instanceImportDto);
//                GetAndSetSpecialFieldAndMainObjectInstanceImportDto(instanceImportDto, objectInstanceGroup.MainObjFieldInfo, instanceGroups, package);
//            }
//        }

//        /// <summary>
//        /// 对象实例数据行
//        /// </summary>
//        /// <param name="sheet"></param>
//        /// <param name="objectConfig"></param>
//        /// <param name="result"></param>
//        /// <param name="headerRow"></param>
//        /// <param name="package"></param>
//        /// <param name="companyColumnIndex"></param>
//        /// <param name="objectInstanceGroup"></param>
//        /// <param name="instanceGroups"></param>
//        private async Task PrepareObjectInstancesBySheetRowsAsync(ISheet sheet, BasicObjectConfig objectConfig, ImExportExcelReturnDto result,
//            IRow headerRow, IConfigPackage package, int? companyColumnIndex, ObjectInstanceImportGroupDto objectInstanceGroup, List<ObjectInstanceImportGroupDto> instanceGroups)
//        {
//            var computeStr = "-";
//            var ignoreStr = "&";
//            var isBreakOut = false;
//            for (var rowIndex = 2; rowIndex <= sheet.LastRowNum; rowIndex++)
//            {
//                var instanceId = SeqGuidHelper.NewGuid();
//                //var uniqueCodes = new List<string>();
//                //var refFieldCodes = new List<string>();
//                var instanceImportDto = new ObjectInstanceImportDto
//                {
//                    Id = instanceId,
//                    ObjectId = objectConfig.Id,
//                    ObjectName = objectConfig.Name,
//                    ObjectCode = objectConfig.Code,
//                    ConfigPackageId = objectConfig.ConfigPackage.Id,
//                    //构造一个 jsonObject
//                    JsonJObject = new JObject
//                    {
//                        { $"Id", JToken.FromObject(instanceId) },
//                        { $"ObjectId", JToken.FromObject(objectConfig.Id) },
//                        { $"ObjectCode", JToken.FromObject(objectConfig.Code) },
//                        { $"ObjectName", JToken.FromObject(objectConfig.Name) },
//                        { $"ConfigPackageId", JToken.FromObject(objectConfig.ConfigPackage.Id) }
//                    }
//                };
//                var dataRow = sheet.GetRow(rowIndex);
//                if (dataRow is null)
//                {
//                    result.Message = $"表单【{sheet.SheetName}】第{rowIndex + 1}行为空，导入结束。总共导入{rowIndex - 2}行";
//                    result.Messages.Add(result.Message);
//                    break;
//                }
//                var lastCellNum = headerRow.LastCellNum;
//                for (var columnIndex = 0; columnIndex < lastCellNum; columnIndex++)
//                {
//                    #region cellValue
//                    var dataRowCell = dataRow.GetCell(columnIndex);
//                    var cellValue = string.Empty;
//                    try
//                    {
//                        if (dataRowCell != null)
//                        {
//                            cellValue = dataRowCell.CellType == CellType.Numeric ? $"{GetCellTextValue(dataRowCell)}" : dataRowCell.StringCellValue;
//                        }
//                    }
//                    catch (Exception exp)
//                    {
//                        result.HasError = true;
//                        result.Message = $"表单【{sheet.SheetName}】第{rowIndex + 1}行,第{columnIndex + 1}列 忽略导入计算cellValue出错:{exp.Message}";
//                        result.ErrorMessages.Add(result.Message);
//                    }

//                    #endregion

//                    #region 前三列 跳出条件

//                    if (columnIndex <= 2)
//                    {
//                        switch (columnIndex)
//                        {
//                            case 0:
//                                continue;
//                            case 1:
//                                if (cellValue.HasValue())
//                                {
//                                    instanceImportDto.CustomReferenceObjectId = cellValue;
//                                }
//                                else if (objectInstanceGroup.MainObjFieldInfo != null)
//                                {
//                                    result.Message = $"表单【{sheet.SheetName}】第{rowIndex + 1}行,第{columnIndex + 1}列无关联字段Id数据，导入结束;总共导入{rowIndex - 2}行";
//                                    result.Messages.Add(result.Message);
//                                    isBreakOut = true;
//                                }
//                                break;
//                            default:
//                                {
//                                    if (!cellValue.HasValue())
//                                    {
//                                        result.Message = $"表单【{sheet.SheetName}】第{rowIndex + 1}行,第{columnIndex + 1}列无Id数据，导入结束;总共导入{rowIndex - 2}行";
//                                        result.Messages.Add(result.Message);
//                                        isBreakOut = true;
//                                    }
//                                    else
//                                    {
//                                        instanceImportDto.CustomId = cellValue;
//                                    }

//                                    break;
//                                }
//                        }

//                        if (isBreakOut)
//                        {
//                            break;
//                        }

//                        continue;
//                    }

//                    #endregion

//                    #region 后续列
//                    if (columnIndex >= headerRow.Cells.Count)
//                    {
//                        break;
//                    }
//                    var headerRowCellValue = GetCellTextValue(headerRow.Cells[columnIndex]);//.StringCellValue;
//                    var fieldCode = headerRowCellValue.Split(computeStr).Last();
//                    var isRefObjField = fieldCode.Contains(ignoreStr);
//                    var fieldInfo = isRefObjField ? GetFieldConfigByUnionCode(fieldCode, ignoreStr, package) : package.GetObjectFieldByCode(objectConfig.Id, fieldCode);
//                    var isUnique = headerRowCellValue.StartsWith($"#");

//                    try
//                    {
//                        if (cellValue.HasValue())
//                        {
//                            if (isRefObjField && fieldInfo.IsIndexed)
//                            {
//                                //当前主对象的引用字段，赋值为从数据库取出来的json对象
//                                var referCodes = fieldCode?.Split(ignoreStr).ToList();
//                                if (referCodes?.Count > 2)
//                                {
//                                    //            { $"Id", JToken.FromObject(instanceId) },
//                                    //{ $"ObjectId", JToken.FromObject(objectConfig.Id) },
//                                    //{ $"ObjectCode", JToken.FromObject(objectConfig.Code) },
//                                    //{ $"ObjectName", JToken.FromObject(objectConfig.Name) },
//                                    //{ $"ConfigPackageId", JToken.FromObject(objectConfig.ConfigPackage.Id) }

//                                    var mainObjFieldCode = referCodes.Last();
//                                    if (!instanceImportDto.DevImportReferCodes.Contains(mainObjFieldCode))
//                                    {
//                                        instanceImportDto.DevImportReferCodes.Add(mainObjFieldCode);
//                                        instanceImportDto.JsonJObject["DevImportReferCodes"] = JArray.FromObject(instanceImportDto.DevImportReferCodes);
//                                    }
//                                    var mainObjFieldConfig = package.GetObjectFieldByCode(objectConfig.Id, mainObjFieldCode);
//                                    var refFieldCode = referCodes.First();
//                                    var refObjectConfig = package.GetObjectById(mainObjFieldConfig.RefObjectId ?? Guid.Empty);
//                                    JToken refInstanceData = null;
//                                    if ((mainObjFieldConfig.IsArray ?? true) && (mainObjFieldConfig.DataType != EnumObjectFieldDataType.父对象))
//                                    {
//                                        var refInstanceArray = new JArray();
//                                        var refInstanceItem = new JObject();
//                                        refInstanceItem.Add("Id", JToken.FromObject(SeqGuidHelper.NewGuid()));
//                                        refInstanceItem.Add("ObjectId", JToken.FromObject(mainObjFieldConfig.RefObjectId));
//                                        refInstanceItem.Add("ObjectCode", JToken.FromObject(refObjectConfig.Code));
//                                        refInstanceItem.Add("ObjectName", JToken.FromObject(refObjectConfig.Name));
//                                        refInstanceItem.Add("ConfigPackageId", JToken.FromObject(refObjectConfig.ConfigPackage.Id));
//                                        refInstanceItem.Add(refFieldCode, FormatCellValueString(cellValue));
//                                        refInstanceItem.Add("DevImportUniqueCodes", JArray.FromObject(new List<string> { refFieldCode }));
//                                        refInstanceArray.Add(refInstanceItem);
//                                        refInstanceData = refInstanceArray;
//                                    }
//                                    else
//                                    {
//                                        var refInstanceItem = new JObject();
//                                        refInstanceItem.Add("Id", JToken.FromObject(SeqGuidHelper.NewGuid()));
//                                        refInstanceItem.Add("ObjectId", JToken.FromObject(mainObjFieldConfig.RefObjectId));
//                                        refInstanceItem.Add("ObjectCode", JToken.FromObject(refObjectConfig.Code));
//                                        refInstanceItem.Add("ObjectName", JToken.FromObject(refObjectConfig.Name));
//                                        refInstanceItem.Add("ConfigPackageId", JToken.FromObject(refObjectConfig.ConfigPackage.Id));
//                                        refInstanceItem.Add(refFieldCode, FormatCellValueString(cellValue));
//                                        refInstanceItem.Add("DevImportUniqueCodes", JArray.FromObject(new List<string> { refFieldCode }));
//                                        refInstanceData = refInstanceItem;
//                                    }
//                                    instanceImportDto.JsonJObject.Add($"{mainObjFieldCode}", refInstanceData);
//                                }
//                            }
//                            else
//                            {
//                                if (isUnique && fieldInfo.IsUnique)
//                                {
//                                    if (!instanceImportDto.DevImportUniqueCodes.Contains(fieldInfo.Code))
//                                    {
//                                        instanceImportDto.DevImportUniqueCodes.Add(fieldInfo.Code);
//                                        instanceImportDto.JsonJObject["DevImportUniqueCodes"] = JArray.FromObject(instanceImportDto.DevImportUniqueCodes);
//                                    }

//                                }

//                                var cellReturnResult = await ValidateFieldAndGetCellValueObj(fieldInfo, dataRowCell,
//                                    cellValue, companyColumnIndex);
//                                if (cellReturnResult.HasError)
//                                {
//                                    if (cellReturnResult.ErrorMessages.Any())
//                                    {
//                                        result.Messages.AddRange(cellReturnResult.ErrorMessages);
//                                    }
//                                }

//                                var cellObj = cellReturnResult.Data;
//                                instanceImportDto.JsonJObject.Add($"{fieldInfo.Code}", JToken.FromObject(cellObj)); //JsonConvert.SerializeObject(cellObj)

//                                //哪些特殊属性需要特殊设置的？ CompanyId PromoterId ResponsibleId Create
//                                instanceImportDto.CompanyId ??= fieldInfo.DataType == EnumObjectFieldDataType.公司列表
//                                    ? (cellObj as List<CompanyFieldValue>)?.FirstOrDefault()?.Id
//                                    : CurrentUser?.LoginCompanyId;
//                                instanceImportDto.PromoterId ??= fieldInfo.DataType == EnumObjectFieldDataType.用户列表 &&
//                                                                 fieldInfo.Code.StartsWith("Promoter")
//                                    ? (cellObj as List<UserFieldValue>)?.FirstOrDefault()?.Id
//                                    : CurrentUser?.UserId;
//                                instanceImportDto.ResponsibleId ??=
//                                    fieldInfo.DataType == EnumObjectFieldDataType.用户列表 &&
//                                    fieldInfo.Code.StartsWith("Responsible")
//                                        ? (cellObj as List<UserFieldValue>)?.FirstOrDefault()?.Id.ToString()
//                                        : CurrentUser?.UserId.ToString();

//                            }

//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        result.HasError = true;
//                        result.Message = $"表单【{sheet.SheetName}】第{rowIndex + 1}行,第{columnIndex + 1}列 计算cellValueObj出错:{ex.Message}";
//                        result.ErrorMessages.Add(result.Message);
//                        return;
//                    }

//                    #endregion
//                }

//                if (isBreakOut)
//                {
//                    break;
//                }

//                if (instanceImportDto.CreatorId == Guid.Empty)
//                {
//                    instanceImportDto.CreatorId = CurrentUser?.UserId ?? Guid.Empty;
//                    instanceImportDto.CreatedTime = DateTime.Now;
//                }

//                objectInstanceGroup.ObjectInstanceImportDtos.Add(instanceImportDto);
//                GetAndSetSpecialFieldAndMainObjectInstanceImportDto(instanceImportDto, objectInstanceGroup.MainObjFieldInfo, instanceGroups, package);

//            }
//        }

//        /// <summary>
//        /// 设置主对象引用字段值
//        /// </summary>
//        /// <param name="objectInstanceImportDto"></param>
//        /// <param name="mainFieldConfig"></param>
//        /// <param name="instanceGroups"></param>
//        /// <param name="package"></param>
//        /// <returns></returns>
//        private ObjectInstanceImportDto GetAndSetSpecialFieldAndMainObjectInstanceImportDto(ObjectInstanceImportDto objectInstanceImportDto, BasicObjectFieldConfig mainFieldConfig, List<ObjectInstanceImportGroupDto> instanceGroups, IConfigPackage package)
//        {
//            //objectInstanceImportDto.JsonJObject["DevImportUniqueCodes"] = JArray.FromObject(objectInstanceImportDto.DevImportUniqueCodes);
//            //objectInstanceImportDto.JsonJObject["DevImportReferCodes"] = JArray.FromObject(objectInstanceImportDto.DevImportReferCodes);

//            #region StatusId
//            var statusId = objectInstanceImportDto.LifecycleStatusId;
//            if (!statusId.HasValue)
//            {
//                if (objectInstanceImportDto.JsonJObject.ContainsKey("StatusCode"))
//                {
//                    var statusCodeToken = objectInstanceImportDto.JsonJObject["StatusCode"];
//                    var statusCode = statusCodeToken?.ToString();
//                    statusId = package.GetLifecycleStatusList(objectInstanceImportDto.ObjectId)
//                        .FirstOrDefault(x => x.Code == statusCode)?.Id;
//                    statusCodeToken?.Parent?.Remove();
//                }
//                if (objectInstanceImportDto.JsonJObject.ContainsKey("StatusName"))
//                {
//                    var statusNameToken = objectInstanceImportDto.JsonJObject["StatusName"];
//                    if (!statusId.HasValue)
//                    {
//                        var statusName = statusNameToken?.ToString();
//                        statusId = package.GetLifecycleStatusList(objectInstanceImportDto.ObjectId)
//                            .FirstOrDefault(x => x.Name.Cn == statusName)?.Id;
//                    }
//                    statusNameToken?.Parent?.Remove();
//                }
//            }

//            if (statusId.HasValue)
//            {
//                if (objectInstanceImportDto.JsonJObject.ContainsKey("StatusId"))
//                {
//                    objectInstanceImportDto.JsonJObject["StatusId"] = JToken.FromObject(statusId);
//                }
//                else
//                {
//                    objectInstanceImportDto.JsonJObject.Add("StatusId", JToken.FromObject(statusId));
//                }
//            }
//            else
//            {
//                if (objectInstanceImportDto.JsonJObject.ContainsKey("StatusId"))
//                {
//                    objectInstanceImportDto.JsonJObject["StatusId"].Parent.Remove();
//                }
//            }
//            #endregion

//            #region Responsible

//            //Responsible
//            if (objectInstanceImportDto.JsonJObject.ContainsKey("Responsible"))
//            {
//                var responsiblerJson = JArray.FromObject(objectInstanceImportDto.JsonJObject[$"Responsible"]);
//                var responsibleIds = responsiblerJson.Select(user => user.SelectToken("Id").ToString()).ToList();
//                if (objectInstanceImportDto.ResponsibleId.HasValue())
//                {
//                    responsibleIds.AddRange(objectInstanceImportDto.ResponsibleId.Split(","));
//                }
//                responsibleIds = responsibleIds.Distinct().ToList();
//                if (objectInstanceImportDto.JsonJObject.ContainsKey("ResponsibleId"))
//                {
//                    objectInstanceImportDto.JsonJObject[$"ResponsibleId"] = JToken.FromObject(responsibleIds);
//                }
//                else
//                {
//                    objectInstanceImportDto.JsonJObject.Add(new JProperty($"ResponsibleId", JToken.FromObject(responsibleIds)));
//                }
//            }

//            #endregion

//            #region Promoter

//            //Promoter
//            if (objectInstanceImportDto.JsonJObject.ContainsKey("Promoter"))
//            {
//                var promoterJson = objectInstanceImportDto.JsonJObject[$"Promoter"];
//                var promoterIds = promoterJson.Select(user => user.SelectToken("Id").ToString()).ToList();
//                if (objectInstanceImportDto.PromoterId.HasValue)
//                {
//                    promoterIds.Add(objectInstanceImportDto.PromoterId.ToString());
//                }
//                promoterIds = promoterIds.Distinct().ToList();
//                if (objectInstanceImportDto.JsonJObject.ContainsKey("PromoterId"))
//                {
//                    objectInstanceImportDto.JsonJObject[$"PromoterId"] = JToken.FromObject(promoterIds);
//                }
//                else
//                {
//                    objectInstanceImportDto.JsonJObject.Add(new JProperty($"PromoterId", JToken.FromObject(promoterIds)));
//                }
//            }

//            #endregion

//            if (string.IsNullOrEmpty(objectInstanceImportDto.CustomReferenceObjectId))
//            {
//                return null;
//            }

//            var mainObjInstanceGroup = instanceGroups.First(x => x.ObjGroupOrder == 1);
//            var mainObjectInstanceImportDto = mainObjInstanceGroup?.ObjectInstanceImportDtos.FirstOrDefault(x => x.CustomId == objectInstanceImportDto.CustomReferenceObjectId);

//            if (mainObjectInstanceImportDto != null)
//            {
//                if (mainFieldConfig != null)
//                {
//                    if (!mainObjectInstanceImportDto.DevImportReferCodes.Contains(mainFieldConfig.Code))
//                    {
//                        mainObjectInstanceImportDto.DevImportReferCodes.Add(mainFieldConfig.Code);
//                        mainObjectInstanceImportDto.JsonJObject["DevImportReferCodes"] = JArray.FromObject(mainObjectInstanceImportDto.DevImportReferCodes);
//                    }

//                    objectInstanceImportDto.IsReferedByOtherInstance = true;
//                    if (mainObjectInstanceImportDto.JsonJObject.ContainsKey($"{mainFieldConfig.Code}"))
//                    {
//                        var refFieldJsonObj = mainObjectInstanceImportDto.JsonJObject[$"{mainFieldConfig.Code}"];
//                        if (refFieldJsonObj != null)
//                        {
//                            if (mainFieldConfig.IsArray == true)
//                            {
//                                var refFieldArray = JArray.FromObject(refFieldJsonObj);
//                                refFieldArray?.Add(JToken.FromObject(objectInstanceImportDto.JsonJObject));
//                                mainObjectInstanceImportDto.JsonJObject[$"{mainFieldConfig.Code}"] = JToken.FromObject(refFieldArray);
//                            }
//                            else
//                            {
//                                LogHelper.Info($"关联主对象实例Id:{objectInstanceImportDto.CustomReferenceObjectId} 主对象引用组非数组类型，但出现了多次引用，以最后一个引用实例Id:{objectInstanceImportDto.CustomId}建立关联关系.");
//                                mainObjectInstanceImportDto.JsonJObject[$"{mainFieldConfig.Code}"] = JToken.FromObject(objectInstanceImportDto.JsonJObject);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        mainObjectInstanceImportDto.JsonJObject.Add(new JProperty($"{mainFieldConfig.Code}", mainFieldConfig.IsArray == true
//                            ? JToken.FromObject(new List<object> { objectInstanceImportDto.JsonJObject })
//                            : JToken.FromObject(objectInstanceImportDto.JsonJObject)));
//                    }
//                }
//            }
//            else
//            {
//                LogHelper.Info($"当前实例Id:{objectInstanceImportDto.CustomId}_没找到关联主对象实例Id{objectInstanceImportDto.CustomReferenceObjectId} 将被忽略导入");
//            }

//            return mainObjectInstanceImportDto;

//        }


//        /// <summary>
//        /// 设置默认值及保存对象实例数据
//        /// </summary>
//        /// <param name="instanceGroups"></param>
//        /// <param name="package"></param>
//        /// <param name="result"></param>
//        /// <returns></returns>
//        private async Task SetAndSaveInstances(List<ObjectInstanceImportGroupDto> instanceGroups, IConfigPackage package, InputKeyCodeWithDataDto input, ImExportExcelReturnDto result)
//        {
//            var referData = input.Data as ReferDataInputDto;
//            var mainReferObjFieldInfo = referData == null ? null : package.GetFieldById(referData.ReferFieldInfo.Id);
//            var mainObjGroup = instanceGroups.First(x => x.ObjGroupOrder == 1);
//            foreach (var instanceGroupDto in instanceGroups.OrderByDescending(x => x.ObjGroupOrder).ToList())
//            {
//                //var checkConfigs = package.GetObjectRepeatCheckByObjectId(instanceGroupDto.ObjectInfo.Id)
//                //    .Where(x => !x.IsSubmit)
//                //    .ToList();
//                foreach (var objectInstanceImportDto in instanceGroupDto.ObjectInstanceImportDtos)
//                {
//                    //var instance = ToDomainDto(objectInstanceImportDto);
//                    //instance.TenantGuid = objConfig.TenantGuid;
//                    //_objectInstanceRepository.Add(instance);
//                    //objectInstanceImportDto.JsonJObject.Add(new JProperty($"TenantGuid", objConfig.TenantGuid));
//                    try
//                    {
//                        #region 非主对象 没找到关联实例Id数据不允许导入数据库

//                        if (instanceGroupDto.ObjGroupOrder > 1 && objectInstanceImportDto.CustomReferenceObjectId.HasValue())
//                        {
//                            var mainObjDto = mainObjGroup.ObjectInstanceImportDtos.FirstOrDefault(x =>
//                                x.CustomId == objectInstanceImportDto.CustomReferenceObjectId);
//                            if (mainObjDto is null)
//                            {
//                                result.Message = $"当前实例Id:{objectInstanceImportDto.CustomId}_没找到关联主对象实例Id{objectInstanceImportDto.CustomReferenceObjectId} 将被忽略导入";
//                                result.Messages.Add(result.Message);
//                                instanceGroupDto.ErrorCount++;
//                                continue;
//                            }
//                        }

//                        #endregion

//                        #region 判断唯一规则与重复规则与对象创建是一个漫长的过程 应该启动一个后台任务去处理

//                        //1、其它更多固定属性 JsonJObject 是否需要创建日期等属性  CompanyId PromoterId ResponsibleId Create?
//                        //2、字段唯一性检查 唯一性配置 对象配置的重复规则处理
//                        //3、已存在对象实例数据更新逻辑？判断的性能-- 

//                        // Create等部分特殊字段不用赋值 导入模板配置为准
//                        //if (objectInstanceImportDto.CompanyId.HasValue &&
//                        //    !objectInstanceImportDto.JsonJObject.ContainsKey("CompanyId"))
//                        //{
//                        //    objectInstanceImportDto.JsonJObject.Add("CompanyId",
//                        //        JToken.FromObject(objectInstanceImportDto.CompanyId));
//                        //}

//                        ////Responsible
//                        //if (objectInstanceImportDto.JsonJObject.ContainsKey("Responsible"))
//                        //{
//                        //    var responsiblerJson = JArray.FromObject(objectInstanceImportDto.JsonJObject[$"Responsible"]);
//                        //    var responsibleIds = responsiblerJson.Select(user => user.SelectToken("Id").ToString()).ToList();
//                        //    if (objectInstanceImportDto.ResponsibleId.HasValue())
//                        //    {
//                        //        responsibleIds.AddRange(objectInstanceImportDto.ResponsibleId.Split(","));
//                        //    }
//                        //    responsibleIds = responsibleIds.Distinct().ToList();
//                        //    if (objectInstanceImportDto.JsonJObject.ContainsKey("ResponsibleId"))
//                        //    {
//                        //        objectInstanceImportDto.JsonJObject[$"ResponsibleId"] = JToken.FromObject(responsibleIds);
//                        //    }
//                        //    else
//                        //    {
//                        //        objectInstanceImportDto.JsonJObject.Add(new JProperty($"ResponsibleId", JToken.FromObject(responsibleIds)));
//                        //    }
//                        //    //var responsibles = this._organizationCacheService.GetOrgUserDtosById(string.Join(',',objectInstanceImportDto.ResponsibleId));
//                        //    //objectInstanceImportDto.JsonJObject.Add("Responsible", JToken.FromObject(UserInputDto.Map(responsibles)));
//                        //}
//                        ////Promoter
//                        //if (objectInstanceImportDto.JsonJObject.ContainsKey("Promoter"))
//                        //{
//                        //    var promoterJson = objectInstanceImportDto.JsonJObject[$"Promoter"];
//                        //    var promoterIds = promoterJson.Select(user => user.SelectToken("Id").ToString()).ToList();
//                        //    if (objectInstanceImportDto.PromoterId.HasValue)
//                        //    {
//                        //        promoterIds.Add(objectInstanceImportDto.PromoterId.ToString());
//                        //    }
//                        //    promoterIds = promoterIds.Distinct().ToList();
//                        //    if (objectInstanceImportDto.JsonJObject.ContainsKey("PromoterId"))
//                        //    {
//                        //        objectInstanceImportDto.JsonJObject[$"PromoterId"] = JToken.FromObject(promoterIds);
//                        //    }
//                        //    else
//                        //    {
//                        //        objectInstanceImportDto.JsonJObject.Add(new JProperty($"PromoterId", JToken.FromObject(promoterIds)));
//                        //    }
//                        //}

//                        //if (objectInstanceImportDto.CreatorId != Guid.Empty &&
//                        //    !objectInstanceImportDto.JsonJObject.ContainsKey("Create"))
//                        //{
//                        //    var creator = _organizationCacheService.GetOrgUserDtoById(objectInstanceImportDto.CreatorId);
//                        //    objectInstanceImportDto.JsonJObject.Add("Create", JToken.FromObject(
//                        //        new Handler(objectInstanceImportDto.CreatorId, objectInstanceImportDto.CreatedTime,
//                        //            creator?.UserName?.Cn)
//                        //        {
//                        //            Account = creator.Account
//                        //        }));
//                        //}


//                        #endregion

//                        #region 对象配置的重复性规则验证
//#if DEBUG

//                        var watch = new Stopwatch();
//                        watch.Start();
//#endif
//                        await _objectRepeatRuleService.Check(new ObjectRepeatCheckDto
//                        {
//                            BasicObjectId = objectInstanceImportDto.ObjectId,
//                            InstanceId = objectInstanceImportDto.Id,
//                            FormValue = objectInstanceImportDto.JsonJObject.ToObject<Dictionary<string, object>>(),
//                            IsSubmit = false
//                        }, CurrentUser);
//#if DEBUG
//                        watch.Stop();
//                        LogHelper.Debug($"实例Id:{objectInstanceImportDto.Id}_{objectInstanceImportDto.CustomId}-->重复性规则校验_objectRepeatRuleService.Check耗时:{watch.Elapsed.Seconds}秒");
//#endif

//                        #endregion

//                        #region 最后插入或更新实例数据到数据库 这里需要防并发与事务处理
//#if DEBUG

//                        watch.Start();
//#endif
//                        await _semaphore.WaitAsync();
//                        await CreateObjectInstance(objectInstanceImportDto).ConfigureAwait(false); //是否去掉await?
//                        instanceGroupDto.SuccessCount++;
//#if DEBUG
//                        watch.Stop();
//                        LogHelper.Debug($"创建实例耗时:{watch.Elapsed.Seconds}秒");
//#endif
//                        #endregion

//                        #region 如果主对象被另一个对象引用，还需要插入这层关系

//                        if (instanceGroupDto.ObjGroupOrder == 1 && mainReferObjFieldInfo != null)
//                        {
//#if DEBUG
//                            watch.Start();
//#endif
//                            await _objectRelationInstanceService.CreateRelationshipAsync(mainReferObjFieldInfo.BasicObjectId, mainReferObjFieldInfo.Id,
//                                referData.ParentInstanceId.Value, mainReferObjFieldInfo.RefObjectId.Value, objectInstanceImportDto.Id);
//#if DEBUG
//                            watch.Stop();
//                            LogHelper.Debug($"实例Id:{objectInstanceImportDto.Id}_{objectInstanceImportDto.CustomId}创建实例关系耗时:{watch.Elapsed.Seconds}秒");
//#endif
//                        }

//                        #endregion
//                    }
//                    catch (AksoException aex)
//                    {
//                        instanceGroupDto.ErrorCount++;
//                        result.Message += $"对象重复规则验证失败:{aex.MessageLanguage} 忽略当前对象实例导入";
//                        result.Messages.Add(result.Message);
//                    }
//                    catch (Exception ex)
//                    {
//                        instanceGroupDto.ErrorCount++;
//                        result.Message += $"插入对象实例错误:{ex.Message},忽略异常继续后续对象实例导入.";
//                        result.Messages.Add(result.Message);
//                        LogHelper.Error($"{result.Message}");
//                    }
//                    finally
//                    {
//                        _semaphore.Release();
//                    }
//                }

//                result.Message +=
//                    $"对象【{instanceGroupDto.ObjectInfo.Name}】实例总共{instanceGroupDto.ObjectInstanceImportDtos.Count}条，导入成功{instanceGroupDto.SuccessCount}条，失败{instanceGroupDto.ErrorCount}条;\n";
//            }
//        }


//        /// <summary>
//        /// 设置默认值及保存对象实例数据
//        /// </summary>
//        /// <param name="instanceGroups"></param>
//        /// <param name="package"></param>
//        /// <param name="result"></param>
//        /// <returns></returns>
//        private async Task<Guid> CreateSaveInstanceTaskAsync(List<ObjectInstanceImportGroupDto> instanceGroups, IConfigPackage package, InputKeyCodeWithDataDto input, ImExportExcelReturnDto result)
//        {
//            //var referData = input.Data as ReferDataInputDto;
//            //var mainReferObjFieldInfo = referData == null ? null : package.GetFieldById(referData.ReferFieldInfo.Id);
//            var mainObjGroup = instanceGroups.FirstOrDefault(x => x.ObjGroupOrder == 1);

//            try
//            {
//                var mainTaskId = SeqGuidHelper.NewGuid();
//                var mainTaskEntity = new ObjectInstanceImportTask
//                {
//                    Id = mainTaskId,
//                    CompanyId = CurrentUser.LoginCompanyId,
//                    TaskNo = $"IMPORT{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{CommonHelper.RandomPassword(6)}",
//                    FilePath = result.FileKey,
//                    FileName = result.FileName,
//                    ObjectId = mainObjGroup.ObjectInfo.Id,
//                    ObjectCode = mainObjGroup.ObjectInfo.Code,
//                    ObjectNameCn = mainObjGroup.ObjectInfo.Name,
//                    PackageId = package.Id,
//                    TaskStatus = EnumInstanceImportTaskStatus.待导入,
//                    Create = new Handler(CurrentHandler.Id, DateTime.Now, CurrentHandler.Name),
//                    Update = new Handler(CurrentHandler.Id, DateTime.Now, CurrentHandler.Name)
//                };
//                var sortNo = 1;
//                var taskRecords = mainObjGroup.ObjectInstanceImportDtos.Select(x => new ObjectInstanceImportTaskRecord
//                {
//                    Id = SeqGuidHelper.NewGuid(),
//                    ImportTaskId = mainTaskId,
//                    SortNum = sortNo++,
//                    JsonData = x.JsonJObject.ToString(),
//                    ExecuteTime = DateTime.Now.AddMinutes(-1),
//                    CreateTime = DateTime.Now,
//                    Status = EnumInstanceImportRecordStatus.待处理
//                }).ToList();
//                using var unitOfWork = _unitOfWorkManager.Begin(true, true);
//                var dbContext = await InstanceImportTaskRepository.GetDbContextAsync();
//                await dbContext.Set<ObjectInstanceImportTask>().AddAsync(mainTaskEntity);
//                await dbContext.BulkInsertAsync<ObjectInstanceImportTaskRecord>(taskRecords);
//                await unitOfWork.CompleteAsync();
//                return mainTaskId;
//            }
//            catch (Exception ex)
//            {
//                LogHelper.Error($"{mainObjGroup.ObjectInfo.Name}对象导入任务创建失败：{ex.ToString()}");
//                throw new AksoException(new LanguageDto { Cn = "导入任务创建失败", En = "fail to create import task" });
//            }

//        }

//        /// <summary>
//        /// 根据JsonJObject 创建对象实例
//        /// </summary>
//        /// <param name="objectInstanceImportDto"></param>
//        /// <returns></returns>
//        private async Task<IObjectInstance> CreateObjectInstance(ObjectInstanceImportDto objectInstanceImportDto)
//        {
//            var objectInstance = await ObjectInstance.CreateInstanceAsync(objectInstanceImportDto.JsonJObject);
//            objectInstance = await _objectInstanceService.SaveObjectInstanceAsync(objectInstance, CurrentUser);
//            if (objectInstance.StatusId.HasValue)
//            {
//#if DEBUG
//                var watch = new Stopwatch();
//                watch.Start();
//#endif
//                await _behaviorEventService.ChangeStatusAsync(objectInstance, objectInstance.StatusId.Value, CurrentUser);
//#if DEBUG
//                watch.Stop();
//                LogHelper.Debug($"导入对象实例{objectInstance?.BizNo}后触发状态改变及执行关联动作耗时:{watch.Elapsed.Seconds}秒");
//#endif
//            }
//            return objectInstance;
//        }

//        /// <summary>
//        /// 唯一判断某字段 是否已存在 
//        /// </summary>
//        /// <param name="objectCode"></param>
//        /// <param name="fieldInfo"></param>
//        /// <param name="cellValue"></param>
//        /// <returns></returns>
//        private async Task<ImExportExcelReturnDto> IsFieldExistAsync(string objectCode, BasicObjectFieldConfig fieldInfo, string cellValue)
//        {
//            ImExportExcelReturnDto result;
//            if (ExistFieldResultDicts.ContainsKey($"{fieldInfo.Id}_{cellValue}"))
//            {
//                result = ExistFieldResultDicts[$"{fieldInfo.Id}_{cellValue}"];
//            }
//            else
//            {
//                var existItems = await _metaDataService.QueryMetaDataAsync(new MetaDataSelectDto
//                {
//                    ObjectCode = objectCode,
//                    SelectFields = new List<string> { "Id" },
//                    FilterGroup = new FiledValueFilterGroup
//                    {
//                        Filters = new List<FieldValueFilter>
//                        {
//                            new FieldValueFilter
//                            {
//                                Key = fieldInfo.Code,
//                                Operate = "=",
//                                Value = cellValue
//                            }
//                        }
//                    }
//                });

//                result = new ImExportExcelReturnDto
//                {
//                    HasError = existItems?.Rows.Count > 0
//                };

//                if (result.HasError)
//                {
//                    result.Data = existItems?.Rows[0][0];
//                    result.Message = $"系统已存在字段{fieldInfo.Code}={cellValue}的数据Id={result.Data?.ToString()}.";
//                    result.ErrorMessages.Add(result.Message);
//                }
//                ExistFieldResultDicts[$"{fieldInfo.Id}_{cellValue}"] = result;
//            }
//            return result;
//        }


//        /// <summary>
//        /// 用户输入字符串格式化
//        /// </summary>
//        /// <param name="cellValue"></param>
//        /// <returns></returns>

//        private static string FormatCellValueString(string cellValue)
//        {
//            if (cellValue.Contains("【"))
//            {
//                cellValue = cellValue.Replace("【", "").Replace("】", "");
//            }
//            if (cellValue.Contains("，"))
//            {
//                cellValue = cellValue.Replace("，", ",");
//            }

//            return cellValue;
//        }


//        /// <summary>
//        /// 名称去特殊字符
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>

//        private static string RemoveSpecialChar(string name)
//        {
//            if (name?.Contains("\\") == true)
//            {
//                name = name.Replace("\\", "");
//            }
//            if (name?.Contains("/") == true)
//            {
//                name = name.Replace("/", "");
//            }
//            if (name?.Contains("_") == true)
//            {
//                name = name.Replace("_", "");
//            }

//            return name;
//        }

//        /// <summary>
//        /// 获取字段值
//        /// </summary>
//        /// <param name="fieldInfo"></param>
//        /// <param name="cell"></param>
//        /// <param name="companyColumnIndex"></param>
//        /// <param name="cellValue"></param>
//        /// <returns></returns>
//        private async Task<ImExportExcelReturnDto> ValidateFieldAndGetCellValueObj(BasicObjectFieldConfig fieldInfo, ICell cell, string cellValue, int? companyColumnIndex = null)
//        {
//            var result = new ImExportExcelReturnDto
//            {
//                Message = $"表单【{cell.Sheet.SheetName}】第{cell.RowIndex + 1}行,第{cell.ColumnIndex + 1}列"
//            };
//            var companyId = CurrentUser?.LoginCompanyId ?? Guid.Empty;
//            if (companyColumnIndex.HasValue)
//            {
//                var companyCode = FormatCellValueString(cell.Row.GetCell(companyColumnIndex.Value).StringCellValue).Split(",").First();
//                var company = await _organizationCacheService.GetOrgCompanyByCodeAsync(companyCode);
//                companyId = company.Id;
//            }
//            switch (fieldInfo?.DataType)
//            {
//                case EnumObjectFieldDataType.用户列表:
//                case EnumObjectFieldDataType.用户组列表:
//                    cellValue = FormatCellValueString(cellValue);
//                    var userAccounts = cellValue.Split(",").ToList();
//                    var users = new List<UserFieldValue>();
//                    foreach (var userAccount in userAccounts)
//                    {
//                        var user = await _organizationCacheService.GetOrgUserDtoByAccountAsync(userAccount);
//                        if (user != null)
//                        {
//                            users.Add(new UserFieldValue(user));
//                        }
//                        else
//                        {
//                            result.HasError = true;
//                            result.Message = $"{result.Message} 用户【{userAccount}】不存在";
//                            result.ErrorMessages.Add(result.Message);
//                        }
//                    }
//                    result.Data = users;
//                    break;
//                case EnumObjectFieldDataType.公司列表:
//                    cellValue = FormatCellValueString(cellValue);
//                    var companies = new List<CompanyFieldValue>();
//                    var companyCodes = cellValue.Split(",").ToList();
//                    foreach (var companyCode in companyCodes)
//                    {
//                        var company = await _organizationCacheService.GetOrgCompanyByCodeAsync(companyCode);
//                        if (company != null)
//                        {
//                            companies.Add(new CompanyFieldValue(company));
//                        }
//                        else
//                        {
//                            result.HasError = true;
//                            result.Message = $"{result.Message} 公司【{companyCode}】不存在";
//                            result.ErrorMessages.Add(result.Message);
//                        }
//                    }

//                    result.Data = companies;
//                    break;
//                case EnumObjectFieldDataType.部门列表:
//                    cellValue = FormatCellValueString(cellValue);
//                    var departments = new List<DepartmentFieldValue>();
//                    var departmentCodes = cellValue.Split(",").ToList();
//                    foreach (var departmentCode in departmentCodes)
//                    {
//                        var department = await _organizationCacheService.GetOrgDepartmentByCodeAsync(departmentCode, companyId);
//                        if (department != null)
//                        {
//                            departments.Add(new DepartmentFieldValue(department));
//                        }
//                        else
//                        {
//                            result.HasError = true;
//                            result.Message = $"{result.Message} 部门【{departmentCode}】不存在";
//                            result.ErrorMessages.Add(result.Message);
//                        }
//                    }

//                    result.Data = departments;
//                    break;
//                case EnumObjectFieldDataType.职位列表:
//                    cellValue = FormatCellValueString(cellValue);
//                    var posts = new List<PositionFieldValue>();
//                    var postCodes = cellValue.Split(",").ToList();
//                    foreach (var postCode in postCodes)
//                    {
//                        var post = await _organizationCacheService.GetOrgPositionDtoByCodeAsync(postCode, companyId);
//                        if (post != null)
//                        {
//                            posts.Add(new PositionFieldValue(post));
//                        }
//                        else
//                        {
//                            result.HasError = true;
//                            result.Message = $"{result.Message} 职位【{postCode}】不存在";
//                            result.ErrorMessages.Add(result.Message);
//                        }
//                    }
//                    result.Data = posts;
//                    break;
//                case EnumObjectFieldDataType.角色列表:
//                    var roles = new List<RoleFieldValue>();
//                    cellValue = FormatCellValueString(cellValue);
//                    var roleNames = cellValue.Split(",").ToList();
//                    foreach (var roleName in roleNames)
//                    {
//                        var role = await _organizationCacheService.GetOrgRoleDtoByNameAsync(roleName, companyId);
//                        if (role != null)
//                        {
//                            roles.Add(new RoleFieldValue(role));
//                        }
//                        else
//                        {
//                            result.HasError = true;
//                            result.Message = $"{result.Message} 角色【{roleName}】不存在";
//                            result.ErrorMessages.Add(result.Message);
//                        }
//                    }
//                    result.Data = roles;
//                    break;
//                case EnumObjectFieldDataType.LongText:
//                case EnumObjectFieldDataType.文本:
//                    cellValue = FormatCellValueString(cellValue);
//                    //计算StatusName StatusCode 字段数据验证
//                    if (fieldInfo.Code.Equals("StatusCode") || fieldInfo.Code.Equals("StatusName"))
//                    {
//                        //var statusList = fieldInfo.ConfigPackage.GetLifecycleStatusList(fieldInfo.BasicObjectId);
//                        var statusId = fieldInfo.ConfigPackage.GetLifecycleStatusList(fieldInfo.BasicObjectId)
//                            .FirstOrDefault(x => x.Code == cellValue || x.Name?.Cn == cellValue)?.Id;
//                        if (!statusId.HasValue)
//                        {
//                            result.HasError = true;
//                            result.Message = $"{result.Message} 该对象字段【{fieldInfo.Code}】无【{cellValue}】状态的数据";
//                            result.ErrorMessages.Add(result.Message);
//                        }
//                    }
//                    result.Data = cellValue;
//                    break;
//                case EnumObjectFieldDataType.日期:
//                    try
//                    {
//                        result.Data = cell.CellType == CellType.Numeric ? cellValue : DateTime.Parse(cellValue);
//                    }
//                    catch (Exception ex)
//                    {
//                        result.HasError = true;
//                        result.Message = $"{result.Message} 【{fieldInfo.Code}】日期类型不满足 yyyy-MM-dd 或者 yyyy-MM-dd HH:mm:ss格式:{ex.Message}";
//                        result.ErrorMessages.Add(result.Message);
//                    }
//                    break;
//                case EnumObjectFieldDataType.数字:
//                    try
//                    {
//                        result.Data = cell.CellType == CellType.Numeric ? cell.NumericCellValue : double.Parse(cellValue);
//                    }
//                    catch (Exception ex)
//                    {
//                        result.HasError = true;
//                        result.Message = $"{result.Message} 【{fieldInfo.Code}】数字类型不满足数字格式:{ex.Message}";
//                        result.ErrorMessages.Add(result.Message);
//                    }
//                    break;
//                case EnumObjectFieldDataType.布尔:
//                    cellValue = FormatCellValueString(cellValue);
//                    if (cellValue.Equals("是"))
//                    {
//                        result.Data = true;
//                    }
//                    else
//                    if (cellValue.Equals("否"))
//                    {
//                        result.Data = false;
//                    }
//                    else
//                    {
//                        result.HasError = true;
//                        result.Message = $"{result.Message} 【{cellValue}】不满足 【是/否】 选项";
//                        result.ErrorMessages.Add(result.Message);
//                    }
//                    break;
//                case EnumObjectFieldDataType.Link:
//                    cellValue = FormatCellValueString(cellValue);
//                    result.Data = cellValue;
//                    break;
//                case EnumObjectFieldDataType.Picklist:
//                    if (fieldInfo.PicklistId.HasValue)
//                    {
//                        var picklist = await _picklistCacheService.GetById(fieldInfo.PicklistId.Value);
//                        if (picklist?.Options?.Any() == true)
//                        {
//                            cellValue = FormatCellValueString(cellValue);
//                            var items = cellValue.Split(",").ToList();
//                            var allOptions = picklist.Options.Select(x => x.Cn).ToList();
//                            var errorMessage = string.Empty;
//                            foreach (var item in items)
//                            {
//                                if (!allOptions.Contains(item))
//                                {
//                                    errorMessage += $" 选项【{item}】不存在;";
//                                }
//                            }
//                            if (errorMessage.HasValue())
//                            {
//                                result.HasError = true;
//                                result.Message = $"{result.Message} {errorMessage}";
//                                result.ErrorMessages.Add(result.Message);
//                            }
//                            else
//                            {
//                                var options = picklist.Options.Where(x => items.Contains(x.Cn)).ToList();
//                                if (options.Any())
//                                {
//                                    result.Data = fieldInfo.IsArray == true ? options : options.First();//JsonConvert.SerializeObject()
//                                }
//                            }
//                        }
//                    }
//                    break;
//                case EnumObjectFieldDataType.Complex:
//                    //$"请输入自定义对象数据，逗号分隔(,)";
//                    cellValue = FormatCellValueString(cellValue);
//                    result.Data = JsonConvert.DeserializeObject<object>(cellValue);
//                    break;
//                case EnumObjectFieldDataType.对象:
//                case EnumObjectFieldDataType.产品:
//                case EnumObjectFieldDataType.物料:
//                case EnumObjectFieldDataType.供应商:
//                case EnumObjectFieldDataType.客户:
//                case EnumObjectFieldDataType.项目:
//                case EnumObjectFieldDataType.资产:
//                case EnumObjectFieldDataType.批次管理:
//                case EnumObjectFieldDataType.子对象:
//                case EnumObjectFieldDataType.关系:
//                case EnumObjectFieldDataType.附件:
//                case EnumObjectFieldDataType.文件一级类型:
//                case EnumObjectFieldDataType.文件二级类型:
//                case EnumObjectFieldDataType.父对象:
//                case EnumObjectFieldDataType.DocumentType:
//                    cellValue = FormatCellValueString(cellValue);
//                    result.Data = cellValue;
//                    break;
//            }

//            return result;
//        }

//        /// <summary>
//        /// 获取字段输入说明
//        /// </summary>
//        /// <param name="fieldInfo"></param>
//        /// <returns></returns>
//        private async Task<string> GetFieldHelpInputRemark(BasicObjectFieldConfig fieldInfo)
//        {
//            var cellValue = $"{fieldInfo?.DataType.ToString()}";
//            //var companyId = CurrentUser?.LoginCompanyId;
//            //var company = companyId.HasValue
//            //    ? _organizationCacheService.GetOrgCompanyById(companyId.Value)
//            //    : null;
//            switch (fieldInfo?.DataType)
//            {
//                case EnumObjectFieldDataType.用户列表:
//                    cellValue = $"请输入用户登录账号，用逗号(,)分隔 例如：【account1】,【account2】";

//                    break;
//                case EnumObjectFieldDataType.公司列表:
//                    cellValue = $"请输入公司编码，用逗号(,)分隔 例如：【companyCode1】,【companyCode2】";
//                    break;
//                case EnumObjectFieldDataType.部门列表:
//                    cellValue = $"请输入部门编码，用逗号(,)分隔 例如：【departCode1】,【departCode2】";
//                    //if (companyId.HasValue)
//                    //{
//                    //    var departments = _organizationCacheService.GetAllOrgDepartmentsByCompanyId(companyId.Value) ?? new List<OrgDepartment>();
//                    //    cellValue = $"{cellValue} {string.Join(",", departments.Take(5).Select(x => $"【{x.DepartmentCode}】"))}";
//                    //}
//                    //else
//                    //{
//                    //    cellValue = $"{cellValue} 【departCode1】,【departCode2】";
//                    //}
//                    break;
//                case EnumObjectFieldDataType.职位列表:
//                    cellValue = $"请输入职位编码，用逗号(,)分隔 例如：【postCode1】,【postCode2】";
//                    //if (companyId.HasValue)
//                    //{
//                    //    var posts = _organizationCacheService.GetAllPositionsByCompanyId(companyId.Value) ?? new List<OrgPosition>();
//                    //    cellValue = $"{cellValue} {string.Join(",", posts.Take(5).Select(x => $"【{x.PositionCode}】"))}";
//                    //}
//                    //else
//                    //{
//                    //    cellValue = $"{cellValue} 【postCode1】,【postCode2】";
//                    //}
//                    break;
//                case EnumObjectFieldDataType.角色列表:
//                    cellValue = $"请输入角色名称，用逗号(,)分隔 例如：【管理员】,【质量审核员】";
//                    //if (companyId.HasValue)
//                    //{
//                    //    var roles = _organizationCacheService.GetAllOrgRoleByCompanyId(companyId.Value) ?? new List<OrgRole>();
//                    //    cellValue = $"{cellValue} {string.Join(",", roles.Take(5).Select(x => $"【{x.RoleName?.Cn}】"))}";
//                    //}
//                    //else
//                    //{
//                    //    cellValue = $"{cellValue} 【管理员】,【质量审核员】";
//                    //}
//                    break;
//                case EnumObjectFieldDataType.对象:
//                    break;
//                case EnumObjectFieldDataType.产品:
//                    break;
//                case EnumObjectFieldDataType.物料:
//                    break;
//                case EnumObjectFieldDataType.供应商:
//                    break;
//                case EnumObjectFieldDataType.客户:
//                    break;
//                case EnumObjectFieldDataType.项目:
//                    break;
//                case EnumObjectFieldDataType.资产:
//                    break;
//                case EnumObjectFieldDataType.批次管理:
//                    break;
//                case EnumObjectFieldDataType.LongText:
//                case EnumObjectFieldDataType.文本:
//                    cellValue = $"请输入{fieldInfo.Name?.Cn}文本 例如:";
//                    if (fieldInfo.Code.Equals("StatusName"))//内置文本字段
//                    {
//                        var statusList = fieldInfo.ConfigPackage.GetLifecycleStatusList(fieldInfo.BasicObjectId);
//                        if (statusList?.Any() == true)
//                        {
//                            cellValue = $"{cellValue}{string.Join("/", statusList.Select(x => $"【{x.Name?.Cn}】"))}";
//                        }
//                    }
//                    else if (fieldInfo.Code.Equals("StatusCode"))//内置文本字段
//                    {
//                        var statusList = fieldInfo.ConfigPackage.GetLifecycleStatusList(fieldInfo.BasicObjectId);
//                        if (statusList?.Any() == true)
//                        {
//                            cellValue = $"{cellValue}{string.Join("/", statusList.Select(x => $"【{x.Code}】"))}";
//                        }
//                    }
//                    else
//                    {
//                        cellValue = $"{cellValue}【{fieldInfo.Name?.Cn}】";
//                    }
//                    break;
//                case EnumObjectFieldDataType.日期:
//                    cellValue = $"请填写日期，日期格式 yyyy-MM-dd [HH:mm:ss] 例如:{DateTime.Now:yyyy-MM-dd}或{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
//                    break;
//                case EnumObjectFieldDataType.数字:
//                    cellValue = $"请输入数字 例如：10";
//                    break;
//                case EnumObjectFieldDataType.文件一级类型:
//                    break;
//                case EnumObjectFieldDataType.文件二级类型:
//                    break;
//                case EnumObjectFieldDataType.父对象:
//                    break;
//                case EnumObjectFieldDataType.布尔:
//                    cellValue = $"请输入【是/否】 例如:【是】";
//                    break;
//                case EnumObjectFieldDataType.子对象:
//                    break;
//                case EnumObjectFieldDataType.关系:
//                    break;
//                case EnumObjectFieldDataType.附件:
//                    break;
//                case EnumObjectFieldDataType.用户组列表:
//                    break;
//                case EnumObjectFieldDataType.Link:
//                    break;
//                case EnumObjectFieldDataType.Picklist:
//                    cellValue = $"请输入Picklist列表内容";
//                    if (fieldInfo.PicklistId.HasValue)
//                    {
//                        var picklist = await _picklistCacheService.GetById(fieldInfo.PicklistId.Value);
//                        if (picklist?.Options?.Any() == true)
//                        {
//                            if (fieldInfo.IsArray == true)
//                            {
//                                var joinStr = string.Join(",", picklist.Options.Select(x => $"【{x.Cn}】").ToList());
//                                cellValue = $"{cellValue} ，逗号分隔(,) 待选值为:\n{joinStr}";
//                            }
//                            else
//                            {
//                                var joinStr = string.Join("/", picklist.Options.Select(x => $"【{x.Cn}】").ToList());
//                                cellValue = $"{cellValue}  待选值为:\n{joinStr}";
//                            }
//                        }
//                    }
//                    break;
//                case EnumObjectFieldDataType.Complex:
//                    cellValue = $"请输入自定义对象数据，逗号分隔(,)";
//                    break;
//                case EnumObjectFieldDataType.DocumentType:
//                    break;
//            }

//            if (fieldInfo?.IsUnique == true)
//            {
//                cellValue = $"【#唯一验证】{cellValue}";
//            }
//            return cellValue;
//        }

//        /// <summary>
//        /// 设置单元格备注
//        /// </summary>
//        /// <param name="cell"></param>
//        /// <param name="mark"></param>
//        private static async Task SetStyleAndComment(ICell cell, string mark)
//        {
//            if (cell != null)
//            {
//                cell.CellStyle.TopBorderColor = HSSFColor.Red.Index;
//                cell.CellStyle.LeftBorderColor = HSSFColor.Red.Index;
//                cell.CellStyle.RightBorderColor = HSSFColor.Red.Index;
//                cell.CellStyle.BorderDiagonalColor = HSSFColor.Red.Index;
//                var patr = cell.Sheet.CreateDrawingPatriarch();
//                var comment = patr.CreateCellComment(new XSSFClientAnchor());
//                comment.Author = $"Akso";
//                comment.String = new XSSFRichTextString($"{mark}");
//                cell.CellComment = comment;
//            }

//            await Task.CompletedTask;
//        }


//        /// <summary>
//        /// 调用文件服务接口上传文件
//        /// </summary>
//        /// <param name="book"></param>
//        /// <returns>返回文件key</returns>
//        private static async Task<string> SaveWorkbook(IWorkbook book)
//        {
//            var key = $"{Guid.NewGuid()}.xlsx";
//            await using var ms = new NpoiMemoryStream();
//            ms.AllowClose = false;
//            book.Write(ms);
//            ms.Seek(0, SeekOrigin.Begin);
//            await AksoFile.Create(ms, key);
//            ms.AllowClose = true;
//            return key;
//        }


//        /// <summary>
//        /// 根据Unicode 获取引用对象字段信息
//        /// </summary>
//        /// <param name="fieldCode"></param>
//        /// <param name="ignoreStr"></param>
//        /// <param name="package"></param>
//        /// <returns></returns>
//        private BasicObjectFieldConfig GetFieldConfigByUnionCode(string fieldCode, string ignoreStr, IConfigPackage package)
//        {
//            BasicObjectFieldConfig fieldInfo = null;
//            //引用类型字段 忽略验证 【引用对象字段code】&【引用对象code】&【引用对象在主对象上的字段code】
//            var referCodes = fieldCode?.Split(ignoreStr).ToList();
//            if (referCodes?.Count > 2)
//            {
//                var referObjConfig = _objectConfigPackageCacheService.GetObjectByCode(referCodes[1]);
//                fieldInfo = package.GetObjectFieldByCode(referObjConfig.Id, referCodes[0]);
//            }

//            return fieldInfo;
//        }

//        /// <summary>
//        /// 根据唯一字段配置重新分配主对象上和引用对象上的字段 
//        /// </summary>
//        /// <param name="input"></param>
//        private static void ResetFieldConfigs(ExportTemplateConfig input)
//        {
//            if (input?.Fields.Any() == true)
//            {
//                var mainObjInfo = input.Fields.First(x => x.ObjectOrder == 1).ObjectInfo;
//                var mainObjMinOrder = input.Fields.Where(x => x.ObjectOrder == 1).Min(y => y.Order);
//                var refObjFields = input.Fields.Where(x => x.ObjectOrder != 1).ToList();
//                var refObjGroup = refObjFields.GroupBy(m => m.ObjectOrder).ToList();
//                foreach (var fieldInfoConfigs in refObjGroup)
//                {
//                    //所有引用对象上的字段当作主对象字段 生成模板
//                    var isMoveToMain = fieldInfoConfigs.All(x => x.IsUnique);
//                    if (isMoveToMain)
//                    {
//                        foreach (var fieldInfoConfig in fieldInfoConfigs)
//                        {
//                            //fieldInfoConfig.Code=//这个Code规则
//                            fieldInfoConfig.UnionCode = $"{fieldInfoConfig.UnionCode.Replace(".", "&")}";//这个UnionCode 要拿来分组规则定义
//                            fieldInfoConfig.ObjectOrder = 1;
//                            fieldInfoConfig.IsRelationshipField = true;
//                            fieldInfoConfig.OrginObjectInfo = fieldInfoConfig.ObjectInfo;
//                            fieldInfoConfig.ObjectInfo = mainObjInfo;
//                            fieldInfoConfig.Order = mainObjMinOrder / 1000 + (fieldInfoConfig.Order) * 1m / 1000;
//                        }
//                    }
//                }
//            }

//        }

//        #endregion

//        #region 当前登录用户


//        private CurrentUserInfo _currentUser = null;
//        protected CurrentUserInfo CurrentUser
//        {
//            get
//            {
//                if (_currentUser != null) return _currentUser;
//                var httpContext = ServiceLocator.Instance.GetScopeService<IHttpContextAccessor>();
//                if (httpContext?.HttpContext?.User?.Identity?.IsAuthenticated == true)
//                {
//                    var principal = httpContext.HttpContext.User;
//                    var userInfoStr = principal?.Claims?.FirstOrDefault(t => t.Type.Equals("CurrentUserInfo"))?.Value;
//                    if (userInfoStr?.HasValue() == true)
//                    {
//                        var language = httpContext.HttpContext.Request.Headers["Language"].ToString();
//                        _currentUser = JsonConvert.DeserializeObject<CurrentUserInfo>(userInfoStr);
//                        if (_currentUser != null)
//                        {
//                            _currentUser.Language = OrgEnumTypeExtend.StringConvertToEnum(language);
//                        }
//                    }
//                }
//                return _currentUser;
//            }
//            set => _currentUser = value;
//        }

//        /// <summary>
//        /// 当前登录人、时间
//        /// </summary>
//        protected Handler CurrentHandler => CurrentUser != null ? new Handler(CurrentUser.UserId, DateTime.Now, CurrentUser?.UserName?.Cn) : null;



//        #endregion

//        /// <summary>
//        /// 查询导入任务对象列表
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<ObjectInstanceImportTaskQueryObjectListDto>> GetImportTaskObjectListAsync(ObjectInstanceImportTaskQueryDto input)
//        {
//            var query = await FilterQueryAsync(input);
//            var objectIds = await query.Select(x => x.ObjectId).Distinct().ToListAsync();
//            return _objectConfigPackageCacheService
//                .GetObjectsByIds(objectIds)
//                .Select(x => new ObjectInstanceImportTaskQueryObjectListDto
//                {
//                    Id = x.Id,
//                    Code = x.Code,
//                    Name = new LanguageDto(x.Name?.Cn, x.Name?.En)
//                }).ToList();
//        }


//        private async Task<IQueryable<ObjectInstanceImportTask>> FilterQueryAsync(ObjectInstanceImportTaskQueryDto queryDto)
//        {
//            var query = await InstanceImportTaskRepository.GetQueryableAsync();
//            return query
//                .WhereIf(queryDto.ObjectId.HasValue, x => x.ObjectId == queryDto.ObjectId)
//                .WhereIf(queryDto.Status.HasValue, x => x.TaskStatus == queryDto.Status)
//                .WhereIf(!queryDto.ShowAllData, x => x.Create.Id == CurrentUser.UserId);
//        }

//        /// <summary>
//        /// 查询导入任务
//        /// </summary>
//        /// <param name="queryDto"></param>
//        /// <returns></returns>
//        public async Task<PagesData<ObjectInstanceImportTaskListDto>> GetImportTaskPageListAsync(ObjectInstanceImportTaskQueryDto queryDto)
//        {
//            var query = await FilterQueryAsync(queryDto);
//            var page = query
//                .OrderByDescending(x => x.Create.Time)
//                .GetPageData(queryDto.PageIndex, queryDto.PageSize);
//            var result = _mapper.Map<PagesData<ObjectInstanceImportTaskListDto>>(page);
//            return result;
//        }

//        /// <summary>
//        /// 查询导入任务详情
//        /// </summary>
//        /// <param name="queryDto"></param>
//        /// <returns></returns>
//        public async Task<PagesData<ObjectInstanceImportTaskRecordListDto>> GetImportTaskRecordPageListAsync(ObjectInstanceImportTaskRecordQueryDto queryDto)
//        {
//            var querySource = await InstanceImportTaskRecordRepository.GetQueryableAsync();
//            querySource = querySource
//                .Where(x => x.ImportTaskId == queryDto.TaskId)
//                .WhereIf(queryDto.Status.HasValue, x => x.Status == queryDto.Status)
//                .OrderBy(x => x.SortNum);
//            var page = querySource.GetPageData(queryDto.PageIndex, queryDto.PageSize);
//            var result = _mapper.Map<PagesData<ObjectInstanceImportTaskRecordListDto>>(page);
//            return result;
//        }

//        /// <summary>
//        /// 查询导入任务详情
//        /// </summary>
//        /// <param name="queryDto"></param>
//        /// <returns></returns>
//        public async Task<ObjectInstanceImportTaskSummaryDto> GetImportTaskSummaryInfoAsync(InputIdDto queryDto)
//        {
//            var taskInfo = await InstanceImportTaskRepository.GetByIdAsync(queryDto.Id);
//            if (taskInfo == null) throw new AksoException(new LanguageDto { Cn = "未找到任务信息", En = "can not find task info" });
//            var summaryInfo = _mapper.Map<ObjectInstanceImportTaskSummaryDto>(taskInfo);
//            var prepareCount = await InstanceImportTaskRecordRepository.CountAsync(x => x.ImportTaskId == taskInfo.Id && x.Status == EnumInstanceImportRecordStatus.待处理);
//            var executingCount = await InstanceImportTaskRecordRepository.CountAsync(x => x.ImportTaskId == taskInfo.Id && x.Status == EnumInstanceImportRecordStatus.处理中);
//            var successCount = await InstanceImportTaskRecordRepository.CountAsync(x => x.ImportTaskId == taskInfo.Id && x.Status == EnumInstanceImportRecordStatus.成功);
//            var errorCount = await InstanceImportTaskRecordRepository.CountAsync(x => x.ImportTaskId == taskInfo.Id && x.Status == EnumInstanceImportRecordStatus.失败);
//            summaryInfo.PrepareCount = prepareCount;
//            summaryInfo.ExecutingCount = executingCount;
//            summaryInfo.SuccessCount = successCount;
//            summaryInfo.ErrorCount = errorCount;
//            return summaryInfo;
//        }

//        /// <summary>
//        /// 查询所有导入任务详情
//        /// </summary>
//        /// <param name="queryDto"></param>
//        /// <returns></returns>
//        public async Task<ObjectInstanceAllImportTaskSummaryDto> GetAllImportTaskSummaryInfoAsync(ObjectInstanceTaskSummaryQueryDto queryDto)
//        {
//            var summaryInfo = new ObjectInstanceAllImportTaskSummaryDto();
//            var querySource = await InstanceImportTaskRepository.GetQueryableAsync();
//            querySource = querySource
//                .WhereIf(queryDto.ObjectId.HasValue, x => x.ObjectId == queryDto.ObjectId)
//                .WhereIf(!queryDto.ShowAllData, x => x.Create.Id == CurrentUser.UserId);
//            var prepareCount = await querySource.CountAsync(x => x.TaskStatus == EnumInstanceImportTaskStatus.待导入);
//            var executingCount = await querySource.CountAsync(x => x.TaskStatus == EnumInstanceImportTaskStatus.导入中);
//            var completeCount = await querySource.CountAsync(x => x.TaskStatus == EnumInstanceImportTaskStatus.导入完成);
//            summaryInfo.PrepareCount = prepareCount;
//            summaryInfo.ExecutingCount = executingCount;
//            summaryInfo.CompleteCount = completeCount;
//            return summaryInfo;
//        }

//        /// <summary>
//        /// 查询导入任务详情
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<int> GetUncompleteTaskCountAsync(ObjectInstanceImportTaskUserSwitchQueryDto input)
//        {
//            var query = await InstanceImportTaskRepository.GetQueryableAsync();
//            return await query
//                .WhereIf(input.ShowAllData == false, x => x.Create.Id == CurrentUser.UserId)
//                .CountAsync(x => x.TaskStatus != EnumInstanceImportTaskStatus.导入完成);
//        }
//    }
//}
