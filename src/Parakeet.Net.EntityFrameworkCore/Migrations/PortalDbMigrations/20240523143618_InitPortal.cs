using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parakeet.Net.Migrations.PortalDbMigrations
{
    /// <inheritdoc />
    public partial class InitPortal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "parakeet");

            migrationBuilder.CreateTable(
                name: "AbpAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ImpersonatorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImpersonatorUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ImpersonatorTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImpersonatorTenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "integer", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Exceptions = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "integer", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBackgroundJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    JobArgs = table.Column<string>(type: "character varying(1048576)", maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextTryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastTryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAbandoned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Priority = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)15),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpClaimTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    Regex = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    RegexDescription = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpClaimTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "boolean", nullable: false),
                    IsAvailableToHost = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedProviders = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ValueType = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatureValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatureValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLinkUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetTenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpLinkUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(95)", maxLength: 95, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGrants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ParentName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    MultiTenancySide = table.Column<byte>(type: "smallint", nullable: false),
                    Providers = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    StateCheckers = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSecurityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Identity = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Action = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSecurityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettingDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    IsVisibleToClients = table.Column<bool>(type: "boolean", nullable: false),
                    Providers = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsInherited = table.Column<bool>(type: "boolean", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettingDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProviderKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserDelegations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserDelegations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SecurityStamp = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ShouldChangePasswordOnNextLogin = table.Column<bool>(type: "boolean", nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false),
                    LastPasswordChangeTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    IdCardNo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true, comment: "身份证号码"),
                    UserStatus = table.Column<int>(type: "integer", nullable: true, comment: "用户状态"),
                    UserType = table.Column<int>(type: "integer", nullable: true, comment: "用户类型"),
                    Sex = table.Column<int>(type: "integer", nullable: true, comment: "性别"),
                    Level = table.Column<int>(type: "integer", nullable: true, comment: "等级"),
                    LastLoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "最近登陆时间"),
                    BirthDay = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "生日"),
                    SignAccountId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "签章的SignAccountId值"),
                    Signature = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: true, comment: "个人签章照片-Base64"),
                    Motto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "个性签名"),
                    HeadPortraitKey = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: true, comment: "头像图片Key或Base64String"),
                    HeadPortraitImage = table.Column<byte[]>(type: "bytea", nullable: true, comment: "头像图片二进制"),
                    IsRealName = table.Column<bool>(type: "boolean", nullable: true, comment: "是否完成实名认证"),
                    IsCompleteGuide = table.Column<bool>(type: "boolean", nullable: true, comment: "是否完成新手引导"),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    ClientType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    JsonWebKeySet = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<string>(type: "text", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedirectUris = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Settings = table.Column<string>(type: "text", nullable: true),
                    ClientUri = table.Column<string>(type: "text", nullable: true),
                    LogoUri = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictScopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Descriptions = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Resources = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Licenses",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppId = table.Column<string>(type: "text", nullable: true, comment: "AppId"),
                    AppKey = table.Column<string>(type: "text", nullable: true, comment: "AppKey"),
                    AppSecret = table.Column<string>(type: "text", nullable: true, comment: "AppId"),
                    Token = table.Column<string>(type: "text", nullable: true, comment: "Token"),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "过期时间"),
                    Name = table.Column<string>(type: "text", nullable: true, comment: "名称"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Licenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_LocationAreas",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true, comment: "父级区域Id"),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, comment: "区域代码"),
                    ParentCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, comment: "父级区域代码"),
                    ZipCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true, comment: "邮编"),
                    Level = table.Column<int>(type: "integer", nullable: false, comment: "区域深度"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "区域名称"),
                    FuallName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "区域全名"),
                    InternationalName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "英文全名"),
                    ShortName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true, comment: "英文缩写"),
                    Pinyin = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true, comment: "拼音"),
                    Prefix = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "拼音首拼"),
                    Longitude = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "经度"),
                    Latitude = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "纬度"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_LocationAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_LocationAreas_parakeet_LocationAreas_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Mediators",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "服务端名称"),
                    Area = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false, comment: "服务端区域码"),
                    DeviceType = table.Column<int>(type: "integer", nullable: false, comment: "设备类型"),
                    Host = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "服务地址"),
                    Port = table.Column<int>(type: "integer", nullable: false, comment: "端口"),
                    Uri = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "接口地址"),
                    Protocol = table.Column<int>(type: "integer", nullable: false, comment: "传输协议"),
                    HandlerType = table.Column<int>(type: "integer", nullable: false, comment: "处理类"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Mediators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Needs",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "客户名称"),
                    Sex = table.Column<int>(type: "integer", nullable: true, comment: "性别"),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "手机"),
                    QNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "QQ"),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true, comment: "邮箱"),
                    Requirements = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false, comment: "需求明细"),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, comment: "是否阅读邮件"),
                    ReadTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "阅读邮件时间"),
                    Address_Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_ZipCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_Country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Province = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Area = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Village = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Detail = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Address_FullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Needs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Notifies",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "消息标题"),
                    ContentDetail = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true, comment: "消息明细"),
                    ReceiveTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "接收时间"),
                    NotifyType = table.Column<int>(type: "integer", nullable: false, comment: "消息类型"),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, comment: "消息状态"),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: true, comment: "发送者"),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: true, comment: "接收者"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true, comment: "项目"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "组织"),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "申请"),
                    IsRequest = table.Column<bool>(type: "boolean", nullable: false, comment: "是否申请"),
                    LinkStatus = table.Column<bool>(type: "boolean", nullable: false, comment: "连接状态"),
                    LinkDetail = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, comment: "连接辅助消息"),
                    SourceType = table.Column<int>(type: "integer", nullable: true, comment: "源头数据类型"),
                    TargetType = table.Column<int>(type: "integer", nullable: true, comment: "目标数据类型"),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: true, comment: "源头"),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: true, comment: "目标"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Notifies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_SecurePolicies",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SecureSourceId = table.Column<Guid>(type: "uuid", nullable: true, comment: "数据源Id"),
                    SecureSourceType = table.Column<int>(type: "integer", nullable: false, comment: "数据源类型"),
                    SecureValidateType = table.Column<int>(type: "integer", nullable: false, comment: "安全策略验证类型,验证ip/clientos/browser/deviceId 默认不验证"),
                    Ips = table.Column<string>(type: "text", nullable: true, comment: "特殊Ip地址集"),
                    StartIpString = table.Column<string>(type: "text", nullable: true, comment: "开始Ip"),
                    EndIpString = table.Column<string>(type: "text", nullable: true, comment: "结束Ip"),
                    StartIpLong = table.Column<long>(type: "bigint", nullable: false, comment: "开始Ip"),
                    EndIpLong = table.Column<long>(type: "bigint", nullable: false, comment: "结束Ip"),
                    ClientOs = table.Column<string>(type: "text", nullable: true, comment: "客户端系统 windows ios andorid"),
                    Browser = table.Column<string>(type: "text", nullable: true, comment: " 移动/PC端浏览器、IE/edge、Chrome"),
                    DeviceId = table.Column<string>(type: "text", nullable: true, comment: "受信任设备，加域或者安装了客户端管理软件的 设备Id"),
                    IsEnable = table.Column<bool>(type: "boolean", nullable: false, comment: "启用true/禁用false 默认为false 禁用"),
                    IsAllow = table.Column<bool>(type: "boolean", nullable: false, comment: "允许true/拒绝false 默认为false 拒绝"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_SecurePolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Thresholds",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "设备名称"),
                    DeviceType = table.Column<int>(type: "integer", nullable: false, comment: "设备类型"),
                    MinAlarmValue = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "报警最小值"),
                    MaxAlarmValue = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "报警最大值"),
                    MinWarningValue = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "预警最小值"),
                    MaxWarningValue = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "预警最大值"),
                    Version = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "版本号"),
                    Factor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Factor 形象因素(字段名称)"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Thresholds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Workers",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdCard = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, comment: "身份证号"),
                    Name = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, comment: "姓名"),
                    Gender = table.Column<int>(type: "integer", nullable: false, comment: " 性别（ 1： 男， 2： 女）"),
                    Nation = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true, comment: "民族"),
                    Birthday = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, comment: "出生日期（ yyyy-MM-dd）"),
                    Address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "住址"),
                    IssuedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "发证机关"),
                    TermValidityStart = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true, comment: "证件有效期起，格式: 20010101"),
                    TermValidityEnd = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true, comment: "证件有效期止，格式: 20010101"),
                    PhoneNumber = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true, comment: "联系电话"),
                    IdPhoto = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: true, comment: "身份证照片（base64）"),
                    Photo = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: true, comment: "现场人员可见光照片（base64）"),
                    InfraredPhoto = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: true, comment: "现场人员红外照片（base64）"),
                    IdPhotoUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, comment: "身份证照片Url"),
                    PhotoUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, comment: "现场人员可见光照片Url"),
                    InfraredPhotoUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, comment: "现场人员红外照片Url"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Workers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_WorkerTypes",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true, comment: "工种编码"),
                    Name = table.Column<string>(type: "text", nullable: true, comment: "工种名称"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_WorkerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpAuditLogActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MethodName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpAuditLogActions_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChangeTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangeType = table.Column<byte>(type: "smallint", nullable: false),
                    EntityTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EntityTypeFullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityChanges_AbpAuditLogs_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AbpAuditLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnitRoles",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnitRoles", x => new { x.OrganizationUnitId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationU~",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnitRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoleClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenantConnectionStrings",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenantConnectionStrings", x => new { x.TenantId, x.Name });
                    table.ForeignKey(
                        name: "FK_AbpTenantConnectionStrings_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserClaims_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLogins",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProviderKey = table.Column<string>(type: "character varying(196)", maxLength: 196, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLogins", x => new { x.UserId, x.LoginProvider });
                    table.ForeignKey(
                        name: "FK_AbpUserLogins_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserOrganizationUnits",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserOrganizationUnits", x => new { x.OrganizationUnitId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationU~",
                        column: x => x.OrganizationUnitId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserOrganizationUnits_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AbpUserTokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictAuthorizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_LicenseResources",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "资源名称"),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "资源Code确保唯一性"),
                    ResourceType = table.Column<int>(type: "integer", nullable: false, comment: "资源类型 1=webApi"),
                    Path = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true, comment: "如果类型为1=webApi，那么将匹配请求资源路由，支持正则表达式，如：^/api.*"),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false, comment: "禁用"),
                    LicenseId = table.Column<Guid>(type: "uuid", nullable: false, comment: "LicenseId"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_LicenseResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_LicenseResources_parakeet_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_AreaTenants",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "区域租户名称，全局唯一"),
                    DisplayName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "区域租户显示名称"),
                    Remark = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true, comment: "备注"),
                    AreaCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, comment: "区域租户码"),
                    LocationAreaId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_AreaTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_AreaTenants_parakeet_LocationAreas_LocationAreaId",
                        column: x => x.LocationAreaId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Organizations",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "名称"),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "代码"),
                    OrganizationType = table.Column<int>(type: "integer", nullable: false, comment: "类型"),
                    Level = table.Column<int>(type: "integer", nullable: false, comment: "层级"),
                    Address_Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_ZipCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_Country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Province = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Area = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Village = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Detail = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Address_FullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    LocationAreaId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域Id"),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true, comment: "父级机构Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_Organizations_parakeet_LocationAreas_LocationAreaId",
                        column: x => x.LocationAreaId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_Organizations_parakeet_Organizations_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Suppliers",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "供应商名称"),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false, comment: "编码"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, comment: "描述"),
                    Phone = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, comment: "联系方式"),
                    Address_Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_ZipCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_Country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Province = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Area = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Village = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Detail = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Address_FullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    LocationAreaId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_Suppliers_parakeet_LocationAreas_LocationAreaId",
                        column: x => x.LocationAreaId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_NeedAttachments",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NeedId = table.Column<Guid>(type: "uuid", nullable: true, comment: "需求外键"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    Attachment_Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Attachment_Content = table.Column<string>(type: "text", nullable: true),
                    Attachment_Bytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    Attachment_Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attachment_Extention = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Attachment_Size = table.Column<decimal>(type: "numeric", nullable: true),
                    Attachment_Path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attachment_VirtualPath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Order = table.Column<decimal>(type: "numeric", nullable: true, comment: "文件顺序")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_NeedAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_NeedAttachments_parakeet_Needs_NeedId",
                        column: x => x.NeedId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Needs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityPropertyChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PropertyTypeFullName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityPropertyChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "AbpEntityChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalTable: "OpenIddictAuthorizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_AreaTenantDbConnectionStrings",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    IsMaster = table.Column<bool>(type: "boolean", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AreaTenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_AreaTenantDbConnectionStrings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_AreaTenantDbConnectionStrings_parakeet_AreaTenants~",
                        column: x => x.AreaTenantId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_AreaTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_OrganizationUsers",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "岗位"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true, comment: "用户"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_OrganizationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_OrganizationUsers_parakeet_Organizations_Organizat~",
                        column: x => x.OrganizationId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Projects",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Period = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "期数"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "名称"),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "开始日期"),
                    PlanEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "计划结束日期"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "结束日期"),
                    Amount = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "总金额"),
                    Percent = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "百分比"),
                    Price = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "单价"),
                    Remark = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true, comment: "备注"),
                    MapPath = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "项目示意图"),
                    Address_Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_ZipCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address_Country = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Province = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Area = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Address_Street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Village = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_Detail = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Address_FullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    LocationAreaId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域Id"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "组织外键"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_Projects_parakeet_LocationAreas_LocationAreaId",
                        column: x => x.LocationAreaId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_Projects_parakeet_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Devices",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "名称"),
                    SerialNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "设备编码"),
                    FakeNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "转发编码"),
                    Type = table.Column<int>(type: "integer", nullable: false, comment: "设备类型"),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "设备唯一密钥"),
                    RegisterNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "备案号"),
                    IsEnable = table.Column<bool>(type: "boolean", nullable: false, comment: "是否启用"),
                    ParentArea = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, comment: "父级区域(省)"),
                    Area = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false, comment: "项目区域(市)"),
                    SubArea = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true, comment: "子区域(区/县)"),
                    LocationAreaId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域Id"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true, comment: "项目Id"),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false, comment: "供应商Id"),
                    ThresholdId = table.Column<Guid>(type: "uuid", nullable: true, comment: "阈值Id"),
                    AreaTenantId = table.Column<Guid>(type: "uuid", nullable: true, comment: "所属租户区域Id"),
                    SequenceId = table.Column<Guid>(type: "uuid", nullable: true, comment: "设备区域序号Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_Devices_parakeet_AreaTenants_AreaTenantId",
                        column: x => x.AreaTenantId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_AreaTenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_Devices_parakeet_LocationAreas_LocationAreaId",
                        column: x => x.LocationAreaId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_Devices_parakeet_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_Devices_parakeet_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parakeet_Devices_parakeet_Thresholds_ThresholdId",
                        column: x => x.ThresholdId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Thresholds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_ProjectAttachments",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true, comment: "项目外键"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    Attachment_Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Attachment_Content = table.Column<string>(type: "text", nullable: true),
                    Attachment_Bytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    Attachment_Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attachment_Extention = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Attachment_Size = table.Column<decimal>(type: "numeric", nullable: true),
                    Attachment_Path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Attachment_VirtualPath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Order = table.Column<decimal>(type: "numeric", nullable: true, comment: "文件顺序")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_ProjectAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_ProjectAttachments_parakeet_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_ProjectUsers",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true, comment: "项目"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true, comment: "用户"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_ProjectUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_ProjectUsers_parakeet_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Sections",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "地块/小区名称"),
                    Address = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false, comment: "小区/地块地址"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, comment: "描述"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true, comment: "项目Id"),
                    LocationAreaId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_Sections_parakeet_LocationAreas_LocationAreaId",
                        column: x => x.LocationAreaId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_Sections_parakeet_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_DeviceAnalogRules",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false, comment: "设备Id"),
                    Period = table.Column<TimeSpan>(type: "interval", nullable: false, comment: "频率/时间间隔"),
                    LastSendTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "最后一次发送数据时间"),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false, comment: "是否启用状态  默认为false"),
                    ExtendUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true, comment: "扩展Url地址"),
                    Remark = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true, comment: "备注"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_DeviceAnalogRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceAnalogRules_parakeet_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_DeviceExtends",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_DeviceExtends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceExtends_parakeet_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_DeviceKeySecrets",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Area = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    SupplierKeyId = table.Column<string>(type: "text", nullable: true),
                    SupplierKeySecret = table.Column<string>(type: "text", nullable: true),
                    ProjectKeyId = table.Column<string>(type: "text", nullable: true),
                    ProjectKeySecret = table.Column<string>(type: "text", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_DeviceKeySecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceKeySecrets_parakeet_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_DeviceMediators",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Forward = table.Column<bool>(type: "boolean", nullable: false),
                    Persist = table.Column<bool>(type: "boolean", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediatorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_DeviceMediators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceMediators_parakeet_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceMediators_parakeet_Mediators_MediatorId",
                        column: x => x.MediatorId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Mediators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_DeviceSequences",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Area = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    DeviceType = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_DeviceSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceSequences_parakeet_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_DeviceWorkers",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProvenceArea = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    IdCardEncrypt = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorpId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorpCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorpName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkerTypeId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkerTypeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkerTypeName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkPostId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkPostCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkPostName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkerGroupId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkerGroupCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    WorkerGroupName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntryStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    JoinDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeaveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttendanceCardId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AttendanceCardNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Mode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    AttendanceCardType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AttendanceCardIssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttendanceCardIssuePic = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GroupLeader = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Marital = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    PoliticsType = table.Column<int>(type: "integer", nullable: false),
                    IsJoin = table.Column<bool>(type: "boolean", nullable: true),
                    JoinTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Education = table.Column<int>(type: "integer", nullable: true),
                    HasBadMedicalHistory = table.Column<bool>(type: "boolean", nullable: false),
                    IsSpecial = table.Column<bool>(type: "boolean", nullable: false),
                    IcCard = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    PersonId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    WorkerNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PersonnelType = table.Column<int>(type: "integer", nullable: false),
                    RegisterType = table.Column<int>(type: "integer", nullable: false),
                    IsOuterRegistered = table.Column<bool>(type: "boolean", nullable: false),
                    IsSendToDevice = table.Column<bool>(type: "boolean", nullable: false),
                    AreaTenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationAreaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_DeviceWorkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceWorkers_parakeet_AreaTenants_AreaTenantId",
                        column: x => x.AreaTenantId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_AreaTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceWorkers_parakeet_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceWorkers_parakeet_LocationAreas_LocationAreaId",
                        column: x => x.LocationAreaId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_LocationAreas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_DeviceWorkers_parakeet_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Houses",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "房间号"),
                    BuildingArea = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "建筑面积"),
                    UseArea = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "使用面积"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, comment: "描述"),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: true, comment: "小区Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Houses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_Houses_parakeet_Sections_SectionId",
                        column: x => x.SectionId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Sections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_SectionWorkers",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "工区名称"),
                    CoverArea = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "面积"),
                    IsTemporary = table.Column<bool>(type: "boolean", nullable: false, comment: "是否临时"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, comment: "描述"),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域/地块Id"),
                    WorkerTypeId = table.Column<Guid>(type: "uuid", nullable: true, comment: "工种Id"),
                    LaborType = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: true, comment: "劳务人员Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_SectionWorkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_SectionWorkers_parakeet_Sections_SectionId",
                        column: x => x.SectionId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Sections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_SectionWorkers_parakeet_WorkerTypes_WorkerTypeId",
                        column: x => x.WorkerTypeId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_WorkerTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_parakeet_SectionWorkers_parakeet_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Workers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_Products",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "名称"),
                    Price = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "单价"),
                    Amount = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "数量"),
                    ChargeType = table.Column<int>(type: "integer", nullable: false, comment: "收费类型"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, comment: "描述"),
                    HouseId = table.Column<Guid>(type: "uuid", nullable: true, comment: "房间Id"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_Products_parakeet_Houses_HouseId",
                        column: x => x.HouseId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_Houses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "parakeet_SectionWorkerDetails",
                schema: "parakeet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "工作位置名称"),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "开始时间"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "结束时间"),
                    Amount = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "数量/工时"),
                    UnitPrice = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "人工单价"),
                    UnitProfit = table.Column<decimal>(type: "numeric(27,3)", nullable: true, comment: "单位利润"),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, comment: "描述"),
                    SectionWorkerId = table.Column<Guid>(type: "uuid", nullable: true, comment: "区域工人"),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parakeet_SectionWorkerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parakeet_SectionWorkerDetails_parakeet_SectionWorkers_Secti~",
                        column: x => x.SectionWorkerId,
                        principalSchema: "parakeet",
                        principalTable: "parakeet_SectionWorkers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_AuditLogId",
                table: "AbpAuditLogActions",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogActions_TenantId_ServiceName_MethodName_Executio~",
                table: "AbpAuditLogActions",
                columns: new[] { "TenantId", "ServiceName", "MethodName", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_UserId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "UserId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_AuditLogId",
                table: "AbpEntityChanges",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_TenantId_EntityTypeFullName_EntityId",
                table: "AbpEntityChanges",
                columns: new[] { "TenantId", "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureGroups_Name",
                table: "AbpFeatureGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_GroupName",
                table: "AbpFeatures",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_Name",
                table: "AbpFeatures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatureValues_Name_ProviderName_ProviderKey",
                table: "AbpFeatureValues",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_Targe~",
                table: "AbpLinkUsers",
                columns: new[] { "SourceUserId", "SourceTenantId", "TargetUserId", "TargetTenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "RoleId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_Code",
                table: "AbpOrganizationUnits",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_ParentId",
                table: "AbpOrganizationUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey",
                table: "AbpPermissionGrants",
                columns: new[] { "TenantId", "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissionGroups_Name",
                table: "AbpPermissionGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_GroupName",
                table: "AbpPermissions",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_Name",
                table: "AbpPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_RoleId",
                table: "AbpRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_NormalizedName",
                table: "AbpRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Action",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_ApplicationName",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "ApplicationName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_Identity",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "Identity" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSecurityLogs_TenantId_UserId",
                table: "AbpSecurityLogs",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettingDefinitions_Name",
                table: "AbpSettingDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_Name_ProviderName_ProviderKey",
                table: "AbpSettings",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_Name",
                table: "AbpTenants",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_NormalizedName",
                table: "AbpTenants",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_UserId",
                table: "AbpUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_LoginProvider_ProviderKey",
                table: "AbpUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "UserId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_RoleId_UserId",
                table: "AbpUserRoles",
                columns: new[] { "RoleId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_Email",
                table: "AbpUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedEmail",
                table: "AbpUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_NormalizedUserName",
                table: "AbpUsers",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_UserName",
                table: "AbpUsers",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictApplications_ClientId",
                table: "OpenIddictApplications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
                table: "OpenIddictAuthorizations",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictScopes_Name",
                table: "OpenIddictScopes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
                table: "OpenIddictTokens",
                columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_AuthorizationId",
                table: "OpenIddictTokens",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ReferenceId",
                table: "OpenIddictTokens",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_AreaTenantDbConnectionStrings_AreaTenantId",
                schema: "parakeet",
                table: "parakeet_AreaTenantDbConnectionStrings",
                column: "AreaTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_AreaTenants_LocationAreaId",
                schema: "parakeet",
                table: "parakeet_AreaTenants",
                column: "LocationAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceAnalogRules_DeviceId",
                schema: "parakeet",
                table: "parakeet_DeviceAnalogRules",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceExtends_DeviceId",
                schema: "parakeet",
                table: "parakeet_DeviceExtends",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceKeySecrets_DeviceId",
                schema: "parakeet",
                table: "parakeet_DeviceKeySecrets",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceMediators_DeviceId",
                schema: "parakeet",
                table: "parakeet_DeviceMediators",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceMediators_MediatorId",
                schema: "parakeet",
                table: "parakeet_DeviceMediators",
                column: "MediatorId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Devices_AreaTenantId",
                schema: "parakeet",
                table: "parakeet_Devices",
                column: "AreaTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Devices_LocationAreaId",
                schema: "parakeet",
                table: "parakeet_Devices",
                column: "LocationAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Devices_ProjectId",
                schema: "parakeet",
                table: "parakeet_Devices",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Devices_SupplierId",
                schema: "parakeet",
                table: "parakeet_Devices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Devices_ThresholdId",
                schema: "parakeet",
                table: "parakeet_Devices",
                column: "ThresholdId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceSequences_DeviceId",
                schema: "parakeet",
                table: "parakeet_DeviceSequences",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceWorkers_AreaTenantId",
                schema: "parakeet",
                table: "parakeet_DeviceWorkers",
                column: "AreaTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceWorkers_DeviceId",
                schema: "parakeet",
                table: "parakeet_DeviceWorkers",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceWorkers_LocationAreaId",
                schema: "parakeet",
                table: "parakeet_DeviceWorkers",
                column: "LocationAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_DeviceWorkers_WorkerId",
                schema: "parakeet",
                table: "parakeet_DeviceWorkers",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Houses_SectionId",
                schema: "parakeet",
                table: "parakeet_Houses",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_LicenseResources_LicenseId",
                schema: "parakeet",
                table: "parakeet_LicenseResources",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_LocationAreas_ParentId",
                schema: "parakeet",
                table: "parakeet_LocationAreas",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_NeedAttachments_NeedId",
                schema: "parakeet",
                table: "parakeet_NeedAttachments",
                column: "NeedId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Organizations_Code_Name_OrganizationType",
                schema: "parakeet",
                table: "parakeet_Organizations",
                columns: new[] { "Code", "Name", "OrganizationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Organizations_LocationAreaId",
                schema: "parakeet",
                table: "parakeet_Organizations",
                column: "LocationAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Organizations_ParentId",
                schema: "parakeet",
                table: "parakeet_Organizations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_OrganizationUsers_OrganizationId",
                schema: "parakeet",
                table: "parakeet_OrganizationUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Products_HouseId",
                schema: "parakeet",
                table: "parakeet_Products",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_ProjectAttachments_ProjectId",
                schema: "parakeet",
                table: "parakeet_ProjectAttachments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Projects_LocationAreaId",
                schema: "parakeet",
                table: "parakeet_Projects",
                column: "LocationAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Projects_OrganizationId",
                schema: "parakeet",
                table: "parakeet_Projects",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_ProjectUsers_ProjectId",
                schema: "parakeet",
                table: "parakeet_ProjectUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Sections_LocationAreaId",
                schema: "parakeet",
                table: "parakeet_Sections",
                column: "LocationAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Sections_ProjectId",
                schema: "parakeet",
                table: "parakeet_Sections",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_SectionWorkerDetails_SectionWorkerId",
                schema: "parakeet",
                table: "parakeet_SectionWorkerDetails",
                column: "SectionWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_SectionWorkers_SectionId",
                schema: "parakeet",
                table: "parakeet_SectionWorkers",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_SectionWorkers_WorkerId",
                schema: "parakeet",
                table: "parakeet_SectionWorkers",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_SectionWorkers_WorkerTypeId",
                schema: "parakeet",
                table: "parakeet_SectionWorkers",
                column: "WorkerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_parakeet_Suppliers_LocationAreaId",
                schema: "parakeet",
                table: "parakeet_Suppliers",
                column: "LocationAreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpAuditLogActions");

            migrationBuilder.DropTable(
                name: "AbpBackgroundJobs");

            migrationBuilder.DropTable(
                name: "AbpClaimTypes");

            migrationBuilder.DropTable(
                name: "AbpEntityPropertyChanges");

            migrationBuilder.DropTable(
                name: "AbpFeatureGroups");

            migrationBuilder.DropTable(
                name: "AbpFeatures");

            migrationBuilder.DropTable(
                name: "AbpFeatureValues");

            migrationBuilder.DropTable(
                name: "AbpLinkUsers");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnitRoles");

            migrationBuilder.DropTable(
                name: "AbpPermissionGrants");

            migrationBuilder.DropTable(
                name: "AbpPermissionGroups");

            migrationBuilder.DropTable(
                name: "AbpPermissions");

            migrationBuilder.DropTable(
                name: "AbpRoleClaims");

            migrationBuilder.DropTable(
                name: "AbpSecurityLogs");

            migrationBuilder.DropTable(
                name: "AbpSettingDefinitions");

            migrationBuilder.DropTable(
                name: "AbpSettings");

            migrationBuilder.DropTable(
                name: "AbpTenantConnectionStrings");

            migrationBuilder.DropTable(
                name: "AbpUserClaims");

            migrationBuilder.DropTable(
                name: "AbpUserDelegations");

            migrationBuilder.DropTable(
                name: "AbpUserLogins");

            migrationBuilder.DropTable(
                name: "AbpUserOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpUserRoles");

            migrationBuilder.DropTable(
                name: "AbpUserTokens");

            migrationBuilder.DropTable(
                name: "OpenIddictScopes");

            migrationBuilder.DropTable(
                name: "OpenIddictTokens");

            migrationBuilder.DropTable(
                name: "parakeet_AreaTenantDbConnectionStrings",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_DeviceAnalogRules",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_DeviceExtends",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_DeviceKeySecrets",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_DeviceMediators",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_DeviceSequences",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_DeviceWorkers",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_LicenseResources",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_NeedAttachments",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Notifies",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_OrganizationUsers",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Products",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_ProjectAttachments",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_ProjectUsers",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_SectionWorkerDetails",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_SecurePolicies",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "AbpEntityChanges");

            migrationBuilder.DropTable(
                name: "AbpTenants");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpRoles");

            migrationBuilder.DropTable(
                name: "AbpUsers");

            migrationBuilder.DropTable(
                name: "OpenIddictAuthorizations");

            migrationBuilder.DropTable(
                name: "parakeet_Mediators",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Devices",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Licenses",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Needs",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Houses",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_SectionWorkers",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "AbpAuditLogs");

            migrationBuilder.DropTable(
                name: "OpenIddictApplications");

            migrationBuilder.DropTable(
                name: "parakeet_AreaTenants",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Suppliers",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Thresholds",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Sections",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_WorkerTypes",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Workers",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Projects",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_Organizations",
                schema: "parakeet");

            migrationBuilder.DropTable(
                name: "parakeet_LocationAreas",
                schema: "parakeet");
        }
    }
}
