//using AutoMapper.QueryableExtensions;
//using Common;
//using Common.Dtos;
//using Common.Entities;
//using Common.Enums;
//using Common.Extensions;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Configuration;
//using Parakeet.Net.LinqExtensions;
//using Serilog;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;
//using Volo.Abp;
//using Volo.Abp.Application.Dtos;
//using Volo.Abp.Caching;
//using Volo.Abp.Domain.Repositories;
//using Volo.Abp.Settings;
//using Volo.Abp.Uow;

//namespace Parakeet.Net.LocationAreas
//{
//    /// <summary>
//    ///     区域服务
//    /// </summary>
//    public class LocationAreaAppService : CustomerAppService, ILocationAreaAppService
//    {
//        //它在内部 序列化/反序列化 缓存对象. 默认使用 JSON 序列化
//        private readonly IDistributedCache<List<LocationAreaDto>> _cacheManager;
//        //private readonly IRepository<LocationArea> _locationAreaRepository;
//        private readonly INetRepository<LocationArea> _locationAreaRepository;
//        //private readonly IUserManager _userManager;
//        private RecordConfig _recordConfig;
//        //private readonly IWebHostEnvironment _env;
//        private readonly LocationAreaExceler _locationAreaExceler;
//        public LocationAreaAppService(
//            //IUserManager userManager,
//            //IOptions<RecordConfig> recordConfig,
//            IDistributedCache<List<LocationAreaDto>> cacheManager,
//            //IRepository<LocationArea> locationAreaRepository,
//            INetRepository<LocationArea> locationAreaRepository,
//            ISettingProvider settingProvider,
//            LocationAreaExceler locationAreaExceler)//, IWebHostEnvironment env
//        {
//            _locationAreaRepository = locationAreaRepository;
//            _locationAreaExceler = locationAreaExceler;
//            _cacheManager = cacheManager;
//            //_userManager = userManager;
//            //_recordConfig = recordConfig.Value;
//            //_env = env;
//        }

//        /// <summary>
//        ///     获取省市区区域列表公共接口 最多带一层子集
//        /// </summary>
//        /// <returns></returns>
//        ///禁用IsMetadataEnabled从而从API Explorer中隐藏此服务, 并且无法被发现. 但是它仍然可以被知道确切API路径/路由的客户端使用
//        //[RemoteService(IsMetadataEnabled = false)]
//        public async Task<IList<LocationAreaDto>> GetLocationAreas(LocationAreaInputDto input)
//        {
//            #region SettingProvider已经默认注入 获取设置中的key对应的value
//            ////Get a value as string.
//            //string userName = await SettingProvider.GetOrNullAsync("Smtp.UserName");
//            ////Get a bool value and fallback to the default value (false) if not set.
//            //bool enableSsl = await _settingProvider.GetAsync<bool>("Smtp.EnableSsl");
//            ////Get a bool value and fallback to the provided default value (true) if not set.
//            //bool enableSsl = await _settingProvider.GetAsync<bool>(
//            //    "Smtp.EnableSsl", defaultValue: true);
//            ////Get a bool value with the IsTrueAsync shortcut extension method
//            //bool enableSsl = await _settingProvider.IsTrueAsync("Smtp.EnableSsl");
//            ////Get an int value or the default value (0) if not set
//            //int port = (await _settingProvider.GetAsync<int>("Smtp.Port"));
//            ////Get an int value or null if not provided
//            //int? port = (await _settingProvider.GetOrNullAsync("Smtp.Port"))?.To<int>();

//            #endregion

//            #region 使用redis缓存 数据量太大,不适合使用redis缓存

//            ////有了level之后就不需要有ParentId了
//            //input.ParentId = input.Level.HasValue ? default(long?) : input.ParentId;
//            var cacheKey = $"{CommonConsts.DefaultDbTablePrefix}{input.Level}{input.ParentId}{input.ParentCode}_Area";
//            var areas = await _cacheManager.GetOrAddAsync(cacheKey, async () =>
//            {
//                var list = await (await _locationAreaRepository.GetQueryableAsync()).AsNoTracking()
//                    .IncludeIf(input.Level.HasValue, m => m.Children)//是否带一层子级
//                    .WhereIf(input.Level.HasValue, m => m.Level == input.Level)
//                    .WhereIf(input.Code.HasValue(), m => m.Code.Equals(input.Code))
//                    .WhereIf(input.ParentId.HasValue, m => m.ParentId == input.ParentId)
//                    .WhereIf(input.ParentCode.HasValue(), m => m.Parent.Code.Equals(input.ParentCode))
//                    .ProjectTo<LocationAreaDto>(Configuration)
//                    .ToListAsync();

//                return list; //ObjectMapper.Map<List<LocationArea>, List<LocationAreaDto>>(list);
//            }, () => new DistributedCacheEntryOptions
//            {
//                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
//            });
//            return areas;

//            #endregion
//        }

//        /// <summary>
//        ///     获取省市区区域列表(分页)
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<IPagedResult<LocationAreaDto>> GetPagedResult(GetLocationAreaListDto input)
//        {
//            return await (await _locationAreaRepository.GetQueryableAsync())
//                .AsNoTracking()
//                .WhereIf(input.Filter.HasValue(), x =>
//                    x.Name.Contains(input.Filter) || x.FuallName.Contains(input.Filter))
//                .IncludeIf(input.Level.HasValue, m => m.Children)//是否带一层子级
//                .WhereIf(input.Level.HasValue, m => m.Level == input.Level)
//                .WhereIf(input.Code.HasValue(), m => m.Code.Equals(input.Code))
//                .WhereIf(input.ParentId.HasValue, m => m.ParentId == input.ParentId)
//                .WhereIf(input.ParentCode.HasValue(), m => m.Parent.Code.Equals(input.ParentCode))
//                .OrderBy(input.Sorting)
//                .ProjectTo<LocationAreaDto>(Configuration)
//                .ToPageResultAsync(input, input.FindTotalCount);
//        }


//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<TreeDto>> GetLocationAreaTree(InputIdsNullDto input)
//        {
//            var items = await (await _locationAreaRepository.GetQueryableAsync())
//                .AsNoTracking()
//                .Select(m => new TreeDto
//                {
//                    Name = m.Name,
//                    ParentId = m.ParentId,
//                    ShowSelectBox = true,
//                    Expanded = false,
//                    Selected = input.Ids.Contains(m.Id),
//                    Key = m.Id.ToString(),
//                    Remark = m.FuallName,
//                    TypeIcon = "list-alt",
//                    Id = m.Id,
//                    DbId = m.Id,
//                    Level = m.Level.ToInt().ToEnum<TreeNodeLevel>(),//(TreeNodeLevel)m.Level
//                }).ToListAsync();
//                ////禁用所有父级节点 注意禁用后先把Expanded设置为true，因为禁用了不能再展开父级节点
//                //foreach (var treeDto in items)
//                //{
//                //    if (items.Any(n => n.ParentId == treeDto.Id))
//                //    {
//                //        treeDto.Disabled = true;
//                //    }
//                //}
//            return items;
//        }

//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public async Task<LoadResult> GetAreaTreeList(DataSourceLoadOptionsBase loadOptions)
//        {
//            var query = (await _locationAreaRepository.GetQueryableAsync())
//                .AsNoTracking()
//                .OrderByDescending(o => o.CreationTime)
//                .ProjectTo<LocationAreaDto>(Configuration);
//            return await DataSourceLoader.LoadAsync(query, loadOptions);
//        }

//        /// <summary>
//        /// 添加实体 提供给devExtreme 前端控件验证实体
//        /// </summary>
//        /// <returns></returns>
//        [Description("添加实体")]
//        public async Task<Guid> InsertAreaTreeList()
//        {
//            var formData = ContextAccessor.HttpContext?.Request.Form["values"];
//            var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationArea>(formData);
//            entity = await _locationAreaRepository.InsertAsync(entity);
//            entity.Parent = entity.ParentId.HasValue
//                ? await _locationAreaRepository.GetAsync(entity.ParentId.Value)
//                : null;
//            entity.ParentCode = entity.Parent?.Code;
//            entity.Level = entity.Parent?.Level + 1 ?? 0;
//            return entity.Id;//ObjectMapper.Map<LocationArea,LocationAreaDto>(entity);
//        }

//        /// <summary>
//        /// UpdateUpdate修改实体 提供给devExtreme,前端控件验证实体
//        /// </summary>
//        /// <returns></returns>
//        [Description("修改实体")]
//        public async Task UpdateAreaTreeList()
//        {
//            var key = GetRequestPrimarykey();
//            var entity = 
//                await (await _locationAreaRepository.GetQueryableAsync())
//                .Include(n => n.Parent)
//                .Include(o => o.Children)
//                .FirstOrDefaultAsync(x=>x.Id==key.Value);
//            Newtonsoft.Json.JsonConvert.PopulateObject(GetFormValuesString(), entity);
//        }

//        /// <summary>
//        /// 根据主键Id删除实体 DeleteDelete 默认提供给devExtreme
//        /// </summary>
//        /// <returns></returns>
//        [Description("删除实体")]
//        public async Task DeleteAreaTreeList()
//        {
//            //找所有的子级然后删除
//            var key = GetRequestPrimarykey();
//            var deleteIds = (await GetAllChildrenByParentId(new InputIdDto { Id = key.Value })).Select(m => m.Id).ToList();
//            await _locationAreaRepository.DeleteAsync(m => deleteIds.Contains(m.Id) || m.Id == key);
//        }


//        /// <summary>
//        /// 提供给devExtreme lookup
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<IList<SelectBox>> GetLocationAreaSelectBox(LocationAreaInputDto input)
//        {
//            var areas = await GetLocationAreas(input);

//            return areas
//                //await _locationAreaRepository.AsNoTracking()
//                //.WhereIf(input.Level.HasValue, m => m.Level == input.Level)
//                //.WhereIf(input.Code.HasValue(), m => m.Code.Equals(input.Code))
//                //.WhereIf(input.ParentId.HasValue, m => m.ParentId == input.ParentId)
//                .Select(m => new SelectBox
//                {
//                    Text = $"{m.ParentName}_{m.Name}",
//                    Value = m.Id
//                }).ToList();
//        }

//        /// <summary>
//        /// 获取父级节点SelectList列表 故意为post
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<SelectBox<string, Guid?>>> GetParentSelectList(InputIdNullDto input)
//        {
//            //不能设置当前节点和自己的子级节点为自己的父级
//            var sonNodeIds = input.Id.HasValue && input.Id != Guid.Empty
//                ? (await GetAllChildrenByParentId(new InputIdDto { Id = input.Id ?? Guid.Empty })).Select(m => m.Id).ToList()
//                : new List<Guid>();
//            var selectNodes = await (await _locationAreaRepository.GetQueryableAsync()).AsNoTracking()
//                .WhereIf(sonNodeIds.Any(), p => !sonNodeIds.Contains(p.Id))
//                .Where(m => m.Id != input.Id)
//                .Select(m => new SelectBox<string, Guid?>
//                {
//                    Text = m.Name,
//                    Value = m.Id
//                }).ToListAsync();
//            if (selectNodes.Any())
//            {
//                selectNodes.AddFirst(new SelectBox<string, Guid?>
//                {
//                    Text = "无",
//                    Value = default
//                });
//            }
//            return selectNodes;
//        }


//        /// <summary>
//        /// 根据当前父级Id获取当前父级下的所有子级
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]
//        public async Task<List<LocationAreaDto>> GetAllChildrenByParentId(InputIdDto input)
//        {
//            var childrens = new List<LocationAreaDto>();
//            var items = await (await _locationAreaRepository.GetQueryableAsync()).AsNoTracking()
//                .Where(m => m.Id == input.Id)
//                .SelectMany(n => n.Children)
//                .Include(x => x.Parent)
//                .Include(x => x.Children)
//                .ProjectTo<LocationAreaDto>(Configuration)
//                .ToListAsync();
//            foreach (var area in items)
//            {
//                childrens.AddRange(await GetAllChildrenByParentId(new InputIdDto { Id = area.Id }));
//            }
//            childrens.AddRange(items);//ObjectMapper.Map<List<LocationArea>, List<LocationAreaDto>>(items)
//            return childrens;
//        }

//        /// <summary>
//        /// 单字段重复性验证
//        /// </summary>
//        /// <param name="input">单个字段选项</param>
//        /// <returns>bool</returns>
//        public async Task<bool> CheckField(FieldCheckOptionInputDto<Guid> input)
//        {
//            var isRepeat = await _locationAreaRepository.AnyAsync();//如果没有数据，默认为false 表示没有重复
//            if (isRepeat)
//            {
//                if (!input.Field.Equal)
//                {
//                    input.Field.Equal = true;
//                }

//                input.Field.Field = input.Field.Field.ToInitialCapitalization();//要把字符串数据都转换为大写
//                var dynamicFieldExpression = ExpressionExtension.DynamicField<LocationArea, Guid>(input.Field);
//                isRepeat = await (await _locationAreaRepository.GetQueryableAsync())
//                    .Where(dynamicFieldExpression)
//                    .AnyAsync();//是否重复
//            }
//            return isRepeat;
//        }

//        /// <summary>
//        /// 多字段重复性验证，dxGrid在新建/编辑数据时 时时验证
//        /// </summary>
//        /// <param name="input">多字段dto选项</param>
//        /// <returns>bool</returns>
//        public async Task<bool> CheckFields(FieldsCheckOptionInputDto<Guid> input)
//        {
//            var query = (await _locationAreaRepository.GetQueryableAsync()).AsNoTracking();
//            var isRepeat = await query.AnyAsync();//如果没有数据，默认为false 前端返回true表示通过
//            if (isRepeat)
//            {
//                foreach (var field in input.Fields)
//                {
//                    field.Field = field.Field.ToInitialCapitalization(); //要把字符串数据都转换为大写
//                    query = query.Where(field.CheckLambda<LocationArea, Guid>());//构造表达式目录树
//                }
//                isRepeat = await query.AnyAsync();//根据条件验证是否重复
//            }
//            return isRepeat;
//        }

//        /// <summary>
//        /// 添加同(Level)区域数据(读取Json数据) 从小到大添加
//        /// </summary>
//        /// <param name="level">0-5</param>
//        /// <returns></returns>
//        public async Task AddLevelAreasInOrder(DeepLevelType level)
//        {
//            #region 直接执行sql
//            //_locationAreaRepository.GetDbContext().Database.ExecuteSqlRaw("sql");
//            #endregion

//            #region 单独读取json文件  一次性读取几万条Json数据到内存 这里需要多长时间？是不是可以再优化？
//            var builder = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("LocationAreas_Cities.json");
//            //var builder = new ConfigurationBuilder()
//            //    .SetBasePath(_env.ContentRootPath)
//            //    //.AddJsonFile("locationAreas.json", optional: false, reloadOnChange: true)
//            //    .AddJsonFile($"locationAreas.json", optional: true)
//            //    .AddEnvironmentVariables();
//            var configuration = builder.Build();
//            Stopwatch watch = new Stopwatch();
//            watch.Start();
//            Log.Logger.Information($"读取Json数据开始............");
//            _recordConfig = configuration.GetSection("RecordConfig").Get<RecordConfig>();
//            watch.Stop();
//            Log.Logger.Information($"读取{_recordConfig.LocationAreas.Count}条数据时间(毫秒)：{watch.ElapsedMilliseconds}....................");
//            var locationAreas = _recordConfig.LocationAreas.Where(m => m.Level == level).ToList();
//            #endregion

//            #region 一、批量插入扩展,超级快,需要提前在模块中注册泛型仓储依赖注入接口

//            //if (locationAreas.Any())
//            //{
//            //    using (var unitOfWork = UnitOfWorkManager.Begin(true))
//            //    {
//            //        var areas = locationAreas.Select(locationArea => new LocationArea(locationArea.Id)
//            //        {
//            //            ParentId = locationArea.ParentId,
//            //            ParentCode = locationArea.ParentCode,
//            //            Code = locationArea.Code,
//            //            ZipCode = locationArea.ZipCode,
//            //            Name = locationArea.Name,
//            //            FuallName = locationArea.Name,
//            //            ShortName = locationArea.ShortName,
//            //            Pinyin = locationArea.Pinyin,
//            //            Prefix = locationArea.Prefix,
//            //            Level = locationArea.Level,
//            //            Longitude = locationArea.Longitude,
//            //            Latitude = locationArea.Latitude
//            //        }).ToList();
//            //        watch.Start();
//            //        Log.Logger.Information($"批量插入{locationAreas.Count}条数据开始..................");
//            //        await _locationAreaRepository.BulkInsert(areas);//PostgreBulkInsert
//            //        await unitOfWork.CompleteAsync();
//            //        watch.Stop();
//            //        Log.Logger.Information($"执行时间(milliseconds)：{watch.ElapsedMilliseconds}................");
//            //    }
//            //}

//            #endregion

//            #region 二、分页插入

//            var count = 5000;
//            for (int i = 0; i <= locationAreas.Count / count; i++)
//            {
//                var areas = locationAreas.Skip(i * count).Take(count).ToList();
//                //每5000条数据做依次数据保存
//                using var unitOfWork = UnitOfWorkManager.Begin(true);
//                foreach (var locationArea in areas)
//                {
//                    await _locationAreaRepository.InsertAsync(new LocationArea(locationArea.Id)
//                    {
//                        ParentId = locationArea.ParentId,
//                        ParentCode = locationArea.ParentCode,
//                        Code = locationArea.Code,
//                        ZipCode = locationArea.ZipCode,
//                        Name = locationArea.Name,
//                        FuallName = locationArea.Name,
//                        ShortName = locationArea.ShortName,
//                        Pinyin = locationArea.Pinyin,
//                        Prefix = locationArea.Prefix,
//                        Level = locationArea.Level,
//                        Longitude = locationArea.Longitude,
//                        Latitude = locationArea.Latitude
//                    });
//                }
//                await unitOfWork.CompleteAsync();
//            }

//            #endregion

//            #region 三、无优化直接插入 超级慢 不要轻易尝试大批量数据插入
//            //foreach (var locationArea in locationAreas)
//            //{
//            //    await _locationAreaRepository.InsertAsync(new LocationArea(locationArea.Id)
//            //    {
//            //        ParentId = locationArea.ParentId,
//            //        ParentCode = locationArea.ParentCode,
//            //        Code = locationArea.Code,
//            //        ZipCode = locationArea.ZipCode,
//            //        Name = locationArea.Name,
//            //        FuallName = locationArea.Name,
//            //        ShortName = locationArea.ShortName,
//            //        Pinyin = locationArea.Pinyin,
//            //        Prefix = locationArea.Prefix,
//            //        Level = locationArea.Level,
//            //        Longitude = locationArea.Longitude,
//            //        Latitude = locationArea.Latitude
//            //    });
//            //}
//            #endregion
//        }

//        /// <summary>
//        /// 数据初始化：导入excel数据文件 读取数据数据库
//        /// </summary>
//        /// <param name="input">多文件上传对象</param>
//        /// <returns></returns>
//        [RemoteService(IsEnabled = true, IsMetadataEnabled = false)]//可以使用但不映射为api
//        public async Task ImportFromExcel(ImportFileDto input)
//        {
//            foreach (var file in input.Files)
//            {
//                //简单导入：1行1条实体ReadDataRowToEntities
//                //复杂导入：时间轴：1行多条实体使用：ReadDataLineToEntities 且传递表头时间行
//                var entities = _locationAreaExceler.ReadDataRowToEntities(new ReadExcelToEntityDto
//                {
//                    File = file,
//                    StartRowIndex = 1
//                    //StartColumnIndex = 0
//                });
//                //对entities做一系列设置,需要按Level先后顺序插入数据库
//                var group = entities.GroupBy(m => m.Level).OrderBy(n => n.Key).ToList();
//                foreach (var locationAreas in group)
//                {
//                    await _locationAreaRepository.BulkInsertAsync(locationAreas.ToList());
//                }
//            }
//        }

//    }
//}