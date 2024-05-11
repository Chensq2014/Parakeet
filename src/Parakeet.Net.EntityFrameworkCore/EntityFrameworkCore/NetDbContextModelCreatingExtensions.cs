using Common;
using Common.CustomAttributes;
using Common.Entities;
using Common.Enums;
using Common.Extensions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Common.Users;
using Volo.Abp;
using Volo.Abp.Domain.Values;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Users;
using Volo.Abp.Identity;
using Volo.Abp.Users.EntityFrameworkCore;

namespace Parakeet.Net.EntityFrameworkCore
{
    /// <summary>
    /// 数据库公共实体字段映射扩展
    /// </summary>
    public static class NetDbContextModelCreatingExtensions
    {
        /// <summary>
        /// 公共实体映射(IdentityServer 和Host都会调用这个共用配置,确保生成相同的数据库表)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="isMigrationDbContext">是否忽略导航属性User映射</param>
        /// <param name="optionsAction"></param>
        public static void ConfigureMain(this ModelBuilder builder, bool isMigrationDbContext = false,
            Action<CustomerModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new CustomerModelBuilderConfigurationOptions();
            optionsAction?.Invoke(options);

            #region 实体映射代码

            builder.Entity<AppUser>(b =>
            {
                //Log.Logger.Information($"AbpIdentityDbProperties.DbTablePrefix：{AbpIdentityDbProperties.DbTablePrefix}");
                //b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "AbpUsers"); //Sharing the same table "AbpUsers" with the IdentityUser
                b.ConfigureByConvention();
                b.ConfigureAbpUser();

                /* Configure mappings for your additional properties
                  * Also see the NetCoreEfCoreEntityExtensionMappings class
                  */
                b.ConfigureCustomUserProperties(); //Moved customization to a method so we can share it with the NetCoreMigrationsDbContext class
            });

            builder.Entity<Notify>(x =>
            {
                x.ToTable(options.TablePrefix + $"Notifies", options.Schema);
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<LocationArea>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(LocationArea)}s", options.Schema);
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<Organization>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(Organization)}s", options.Schema);
                //值类型需要实体mapping设置OwnsOne说明[Address]属性是固有实体,
                //efcore新版本直接再实体上标识[Owned]属性即可
                x.OwnsOne(y => y.Address);
                //x.OwnsOne(y => y.Address, z =>
                //{
                //    z.Property(m => m.Code).HasColumnName("Address_Code");
                //    z.Property(m => m.ZipCode).HasColumnName("Address_ZipCode");
                //    z.Property(m => m.Country).HasColumnName("Address_Country");
                //    z.Property(m => m.Province).HasColumnName("Address_Province");
                //    z.Property(m => m.City).HasColumnName("Address_City");
                //    z.Property(m => m.Area).HasColumnName("Address_Area");
                //    z.Property(m => m.Street).HasColumnName("Address_Street");
                //    z.Property(m => m.Detail).HasColumnName("Address_Detail");
                //    z.Property(m => m.FullName).HasColumnName("Address_FullName");
                //});
                x.HasIndex(m => new { m.Code, m.Name, m.OrganizationType }).IsUnique();//建联合唯一索引
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<OrganizationUser>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(OrganizationUser)}s", options.Schema);
                //if (isMigrationDbContext)
                //{
                //    x.Ignore(y => y.User);
                //}
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<Project>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(Project)}s", options.Schema);
                x.OwnsOne(y => y.Address);
                //x.OwnsOne(y => y.Address, z =>
                //{
                //    z.Property(m => m.Code).HasColumnName("Address_Code");
                //    z.Property(m => m.ZipCode).HasColumnName("Address_ZipCode");
                //    z.Property(m => m.Country).HasColumnName("Address_Country");
                //    z.Property(m => m.Province).HasColumnName("Address_Province");
                //    z.Property(m => m.City).HasColumnName("Address_City");
                //    z.Property(m => m.Area).HasColumnName("Address_Area");
                //    z.Property(m => m.Street).HasColumnName("Address_Street");
                //    z.Property(m => m.Detail).HasColumnName("Address_Detail");
                //    z.Property(m => m.FullName).HasColumnName("Address_FullName");
                //});
                x.ConfigureFullAuditedAggregateRoot();
                //MapEntityProertyComment(x);
            });
            builder.Entity<ProjectUser>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(ProjectUser)}s", options.Schema);
                //x.Property(m => m.ProjectId).HasComment("项目Id");
                x.Property("ProjectId").HasComment("项目Id");
                //if (isMigrationDbContext)
                //{
                //    x.Ignore(y => y.User);//identityUser过来的context忽略某个外键关系
                //}
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<ProjectAttachment>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(ProjectAttachment)}s", options.Schema);
                x.OwnsOne(y => y.Attachment);
                x.ConfigureFullAuditedAggregateRoot();
                MapEntityProertyComment(x);
            });
            builder.Entity<Need>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(Need)}s", options.Schema);
                x.OwnsOne(y => y.Address);
                //x.OwnsOne(y => y.Address, z =>
                //{
                //    z.Property(m => m.Code).HasColumnName("Address_Code");
                //    z.Property(m => m.ZipCode).HasColumnName("Address_ZipCode");
                //    z.Property(m => m.Country).HasColumnName("Address_Country");
                //    z.Property(m => m.Province).HasColumnName("Address_Province");
                //    z.Property(m => m.City).HasColumnName("Address_City");
                //    z.Property(m => m.Area).HasColumnName("Address_Area");
                //    z.Property(m => m.Street).HasColumnName("Address_Street");
                //    z.Property(m => m.Detail).HasColumnName("Address_Detail");
                //    z.Property(m => m.FullName).HasColumnName("Address_FullName");
                //});
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<NeedAttachment>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(NeedAttachment)}s", options.Schema);
                x.OwnsOne(y => y.Attachment);
                x.ConfigureFullAuditedAggregateRoot();
                MapEntityProertyComment(x);
            });
            builder.Entity<Supplier>(x =>
            {
                x.ToTable(options.TablePrefix + $"{nameof(Supplier)}s", options.Schema);
                x.OwnsOne(y => y.Address);
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<Threshold>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(Threshold)}s", options.Schema);
            });
            builder.Entity<AreaTenant>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(AreaTenant)}s", options.Schema);
            });
            builder.Entity<AreaTenantDbConnectionString>(x =>
            {
                //x.ConfigureFullAuditedAggregateRoot();//子表非领域根
                x.ToTable(options.TablePrefix + $"{nameof(AreaTenantDbConnectionString)}s", options.Schema);
            });
            builder.Entity<Device>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(Device)}s", options.Schema);
            });
            builder.Entity<DeviceSequence>(x =>
            {
                //typeBuilder.TryConfigureFullAudited();//子表非领域根
                x.ToTable(options.TablePrefix + $"{nameof(DeviceSequence)}s", options.Schema);
            });
            builder.Entity<DeviceExtend>(x =>
            {
                //typeBuilder.TryConfigureFullAudited();//子表非领域根
                x.ToTable(options.TablePrefix + $"{nameof(DeviceExtend)}s", options.Schema);
            });
            builder.Entity<DeviceKeySecret>(x =>
            {
                //typeBuilder.TryConfigureFullAudited();//子表非领域根
                x.ToTable(options.TablePrefix + $"{nameof(DeviceKeySecret)}s", options.Schema);
            });
            builder.Entity<DeviceMediator>(x =>
            {
                //x.TryConfigureFullAudited();//子表非领域根
                //x.HasKey(mediator => new { mediator.DeviceId, mediator.MediatorId });
                //x.HasOne(y => y.Device).WithMany(z => z.Mediators).HasForeignKey(o => o.DeviceId);
                //x.HasOne(y => y.Mediator).WithMany(z => z.DeviceMediators).HasForeignKey(o => o.MediatorId);
                x.ToTable(options.TablePrefix + $"{nameof(DeviceMediator)}s", options.Schema);
            });
            builder.Entity<DeviceAnalogRule>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();//领域根
                x.ToTable(options.TablePrefix + $"{nameof(DeviceAnalogRule)}s", options.Schema);
            });
            builder.Entity<DeviceWorker>(x =>
            {
                //x.ConfigureFullAuditedAggregateRoot();//子表非领域根
                x.ToTable(options.TablePrefix + $"{nameof(DeviceWorker)}s", options.Schema);
            });
            builder.Entity<Worker>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(Worker)}s", options.Schema);
            });
            builder.Entity<WorkerType>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(WorkerType)}s", options.Schema);
            });
            builder.Entity<Mediator>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(Mediator)}s", options.Schema);
            });
            builder.Entity<Common.Entities.License>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(Common.Entities.License)}s", options.Schema);
            });
            builder.Entity<LicenseResource>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(LicenseResource)}s", options.Schema);
            });
            builder.Entity<Section>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(Section)}s", options.Schema);
            });
            builder.Entity<SectionWorker>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(SectionWorker)}s", options.Schema);
            });
            builder.Entity<SectionWorkerDetail>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(SectionWorkerDetail)}s", options.Schema);
            });
            builder.Entity<House>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(House)}s", options.Schema);
            });
            builder.Entity<Product>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"{nameof(Product)}s", options.Schema);
            });
            builder.Entity<SecurePolicy>(x =>
            {
                x.ConfigureFullAuditedAggregateRoot();
                x.ToTable(options.TablePrefix + $"SecurePolicies", options.Schema);
            });
            #endregion

            //builder.Model.RemoveEntityType(nameof(AuditLog));//忽略
            //builder.Ignore<AuditLog>();//可以忽略某个实体不生成到数据库

            BuilderExtention(builder);
        }

        /// <summary>
        ///      每个租户公共实体映射
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="isMigrationDbContext">是否忽略导航属性User映射</param>
        /// <param name="optionsAction"></param>
        public static void Configure(this ModelBuilder builder, bool isMigrationDbContext = false,
            Action<CustomerModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new CustomerModelBuilderConfigurationOptions();
            optionsAction?.Invoke(options);

            #region 实体映射代码
            //options.TablePrefix = "d_";//当前这四个属于device
            builder.Entity<EnvironmentRecord>(x =>
            {
                //x.ToTable(options.TablePrefix + $"{nameof(EnvironmentRecord)}s");
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<CraneBasic>(x =>
            {
                //x.ToTable(options.TablePrefix + $"{nameof(CraneBasic)}s");
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<CraneRecord>(x =>
            {
                //x.ToTable(options.TablePrefix + $"{nameof(CraneRecord)}s", "device");
                x.ConfigureFullAuditedAggregateRoot();
            });
            builder.Entity<CraneAlarm>(x =>
            {
                //x.ToTable(options.TablePrefix + $"{nameof(CraneAlarm)}s");
                x.ConfigureFullAuditedAggregateRoot();
            });
            #endregion

            BuilderExtention(builder);
        }

        /// <summary>
        /// ModelBuilder扩展
        /// </summary>
        /// <param name="builder"></param>
        private static void BuilderExtention(ModelBuilder builder)
        {
            //var mutableProperties = builder.Model.GetEntityTypes().SelectMany(x => x.GetProperties());//.ToList();
            //UniformNumberPrecision(builder, mutableProperties);
            //PgSqlDateTimeFormat(builder, mutableProperties);//pgsql
            MapEntityComments(builder); //, mutableProperties.Select(y => y.PropertyInfo));
        }

        /// <summary>
        /// 统一小数位数
        /// </summary>
        /// <param name="builder">ModelBuilder</param>
        /// <param name="mutableProperties">类字段属性上的属性集合mutableProperties</param>
        private static void UniformNumberPrecision(ModelBuilder builder, [CanBeNull] IEnumerable<IMutableProperty> mutableProperties = null)
        {
            #region decimal小数位数控制 财务系统保存数据系统统一使用decimal

            ////单个字段小数位数控制
            ////modelBuilder.Entity<Project>().Property(m => m.Price).HasColumnType("decimal(27,9)");
            var properties = (mutableProperties ?? builder.Model.GetEntityTypes().SelectMany(x => x.GetProperties()))
                .Where(x => x.ClrType == typeof(decimal) || x.ClrType == typeof(decimal?));
            foreach (var property in properties)
            {
                property.SetColumnType(CommonConsts.Decimal273); //非财务系统使用3位小数即可
                //property.Relational().ColumnType = "decimal(27, 9)";
            }

            #endregion
        }

        /// <summary>
        /// 统一日期时间戳位数
        /// </summary>
        /// <param name="builder">ModelBuilder</param>
        /// <param name="mutableProperties">类字段属性上的属性集合mutableProperties</param>
        private static void PgSqlDateTimeFormat(ModelBuilder builder, [CanBeNull] IEnumerable<IMutableProperty> mutableProperties = null)
        {
            #region 带时区的datetime 数据库格式 timestamptz(6)

            var properties = (mutableProperties ?? builder.Model.GetEntityTypes().SelectMany(x => x.GetProperties()))
                .Where(x => x.ClrType == typeof(DateTime) || x.ClrType == typeof(DateTime?));
            foreach (var property in properties)
            {
                property.SetColumnType(CommonConsts.Timestamptz); //带时区
            }
            #endregion
        }

        /// <summary>
        /// 映射实体注释,字段类型到migration-->执行migration之后就更新到数据库表和字段里了
        /// </summary>
        /// <param name="builder">ModelBuilder实体与数据库之间关系的的建设者</param>
        /// <param name="properties">所有有description的实体属性集合</param>
        private static void MapEntityComments(ModelBuilder builder, IEnumerable<PropertyInfo> properties = null)
        {
            properties = (properties ?? builder.Model.GetEntityTypes()
                             .Select(m => m.ClrType)
                             .Where(n => n.IsAssignableTo(typeof(BaseEntity)) && !n.IsAssignableTo(typeof(BaseAttachmentEntity)) && !n.IsAssignableTo(typeof(ValueObject)))
                             .SelectMany(x => x.GetProperties()))
                .Where(m => !m.IsCollectible && !m.GetMethod.IsVirtual
                       && m.GetCustomAttribute<NotSetAttribute>() == null && m.GetCustomAttribute<DescriptionAttribute>() != null)
                .ToList();
            Log.Logger.Information($"映射实体字段注释：{properties.Count()}条");
            foreach (var property in properties)
            {
                //if (property.DeclaringType is null) continue;//property.DeclaringType 这里一定不为null
                var typeBuilder = builder.Entity(property.DeclaringType);
                var propotyBuilder = typeBuilder.Property(property.Name);

                #region 统一小数位数

                if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                {
                    //非财务系统使用decimal类型3位小数即可
                    propotyBuilder.HasColumnType(CommonConsts.Decimal273);
                }
                #endregion

                #region 统一时间格式为带时区的时间格式

                ////默认不带时区好了，需要时再加
                //if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                //{
                //    propotyBuilder.HasColumnType(CommonConsts.Timestamptz);
                //}
                #endregion

                #region 默认添加字段注释到数据库

                var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description;
                if (description.HasValue())
                {
                    propotyBuilder.HasComment(description);
                }
                #endregion
            }
        }

        /// <summary>
        /// 指定实体类Comment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeBuilder"></param>
        private static void MapEntityProertyComment<T>(EntityTypeBuilder<T> typeBuilder) where T : BaseEntity
        {
            var properties = typeBuilder.Metadata.ClrType.GetProperties()
                .Where(m => !m.IsCollectible && !m.GetMethod.IsVirtual && m.GetCustomAttribute<NotSetAttribute>() == null && m.GetCustomAttribute<DescriptionAttribute>() != null)
                .ToList();
            foreach (var property in properties)
            {
                var description = property.GetCustomAttribute<DescriptionAttribute>().Description;
                if (description.HasValue())
                {
                    var propotyBuilder = typeBuilder.Property(property.Name);
                    propotyBuilder.HasComment(description);
                }
            }
        }

        /// <summary>
        /// 配置user上的自定义属性 值类型Address除外
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="b"></param>
        public static void ConfigureCustomUserProperties<TUser>(this EntityTypeBuilder<TUser> b)
            where TUser : class, IUser
        {
            b.Property<string>(nameof(AppUser.IdCardNo)).HasMaxLength(CommonConsts.MaxLength18).HasComment("身份证号码");
            b.Property<UserStatus?>(nameof(AppUser.UserStatus)).HasComment("用户状态");
            b.Property<UserType?>(nameof(AppUser.UserType)).HasComment("用户类型");
            b.Property<Sex?>(nameof(AppUser.Sex)).HasComment("性别");
            b.Property<TreeNodeLevel?>(nameof(AppUser.Level)).HasComment("等级");
            b.Property<DateTime?>(nameof(AppUser.LastLoginTime)).HasComment("最近登陆时间");
            b.Property<DateTime?>(nameof(AppUser.BirthDay)).HasComment("生日");
            b.Property<string>(nameof(AppUser.SignAccountId)).HasMaxLength(CommonConsts.MaxLength255).HasComment("签章的SignAccountId值");
            b.Property<string>(nameof(AppUser.Signature)).HasMaxLength(CommonConsts.MaxLength8192).HasComment("个人签章照片-Base64");
            b.Property<string>(nameof(AppUser.Motto)).HasMaxLength(CommonConsts.MaxLength255).HasComment("个性签名");
            b.Property<string>(nameof(AppUser.HeadPortraitKey)).HasMaxLength(CommonConsts.MaxLength8192).HasComment("头像图片Key或Base64String");
            b.Property<byte[]>(nameof(AppUser.HeadPortraitImage)).HasComment("头像图片二进制");
            b.Property<bool?>(nameof(AppUser.IsRealName)).HasComment("是否完成实名认证");
            b.Property<bool?>(nameof(AppUser.IsCompleteGuide)).HasComment("是否完成新手引导");

            #region 特殊处理的Address 仍不能映射
            //b.Property<string>("Address_Code");
            //b.Property<string>("Address_ZipCode");
            //b.Property<string>("Address_Country");
            //b.Property<string>("Address_Province");
            //b.Property<string>("Address_City");
            //b.Property<string>("Address_Area");
            //b.Property<string>("Address_Street");
            //b.Property<string>("Address_Village");
            //b.Property<string>("Address_Detail");
            //b.Property<string>("Address_FullName");
            #endregion
        }
    }
}
