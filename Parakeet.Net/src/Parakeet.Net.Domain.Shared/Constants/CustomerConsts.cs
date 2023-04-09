namespace Parakeet.Net
{
    public static class CustomerConsts
    {
        #region 默认长度
        public const int MaxLength4 = 4;
        public const int MaxLength6 = 6;
        public const int MaxLength8 = 8;
        public const int MaxLength11 = 11;
        public const int MaxLength16 = 16;
        public const int MaxLength18 = 18;
        public const int MaxLength32 = 32;
        public const int MaxLength36 = 36;
        public const int MaxLength64 = 64;
        public const int MaxLength128 = 128;
        public const int MaxLength255 = 255;
        public const int MaxLength512 = 512;
        public const int MaxLength1024 = 1024;
        public const int MaxLength2048 = 2048;
        public const int MaxLength4096 = 4096;
        public const int MaxLength8192 = 8192;
        #endregion

        #region 系统默认
        public const string AppName = "Parakeet";

        /// <summary>
        /// 系统默认的bundle名称
        /// </summary>
        public const string GlobalBundleName = "Basic.Global";

        public const string LocalizationSourceName = "Parakeet";

        public const string ConnectionStringName = "Default";//SqlServer/PgSql/MySql
        public const string SqlServerConnectionStringName = "SqlServer";
        public const string PgSqlConnectionStringName = "PgSql";
        public const string MySqlConnectionStringName = "MySql";

        public const bool MultiTenancyEnabled = true;//多租户开关
        #endregion

        #region 默认格式

        public const string DateFormatString = "yyyy/MM/dd";
        public const string DateTimeFormatForDisplay = "yyyy/MM/dd HH:mm:ss";
        public const string DateTimeFormatString = "yyyy/MM/dd HH:mm:ss.fff";

        #endregion

        #region 定义常量
        /// <summary>
        /// Header OrganizationId属性常量
        /// </summary>
        public const string HeaderOrganizationId = "OrganizationId";

        /// <summary>
        /// Header CompanyId属性常量
        /// </summary>
        public const string HeaderCompanyId = "CompanyId";

        /// <summary>
        /// Header ProjectId属性常量
        /// </summary>
        public const string HeaderProjectId = "ProjectId";
        #endregion

        #region 缓存组 暂未使用

        public const string Redis = "Redis";
        public const string RedisConn = "Redis:Configuration";
        public const string CsRedisConn = "Redis:CsRedisConfiguration";

        /// <summary>
        /// 个性化验证/验证码缓存组
        /// </summary>
        public const string PersonalCaches = "PersonalCaches";
        /// <summary>
        /// UserLocker缓存组
        /// </summary>
        public const string UserLocker = "UserLocker";
        /// <summary>
        /// 省市区列表信息缓存组 
        /// </summary>
        public const string LocationArea = "LocationArea";
        /// <summary>
        /// UserUnReadNotify缓存组
        /// </summary>
        public const string UserUnReadNotify = "UserUnReadNotify";
        #endregion

        public const string DefaultDbTablePrefix = "Parakeet_";
        public const string DefaultDbSchema = null;
        public const string ParakeetSchema = "parakeet";

        public const int MaxEmailAddressLength = 256;

        public const int MaxPlainPasswordLength = 32;

        /// <summary>
        ///     Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 1000;

        /// <summary>
        ///     Maximum allowed page size for paged requests.
        /// </summary>
        public const int MaxPageSize = int.MaxValue;

        /// <summary>
        ///     默认排序
        /// </summary>
        public const string DefaultSorting = "CreationTime desc";

        /// <summary>
        ///     随机数最小值.六位数
        /// </summary>
        public const int MinNumber = 100000;

        /// <summary>
        ///     随机数最大值.六位数
        /// </summary>
        public const int MaxNumber = 999999;

        /// <summary>
        ///     最小值范围
        /// </summary>
        public const int MinValue = 0;

        /// <summary>
        ///     最大值范围
        /// </summary>
        public const double MaxValue = double.MaxValue;

        /// <summary>
        /// 十进制 Decimal 小数长度27，精度9
        /// </summary>
        public const string Decimal279 = "DECIMAL(27, 9)";

        /// <summary>
        /// 十进制 Decimal 小数长度27，精度3
        /// </summary>
        public const string Decimal273 = "DECIMAL(27, 3)";

        /// <summary>
        /// Pgsql时间格式 带时区的 timestamptz (不带时区的 timestamp)
        /// </summary>
        public const string Timestamptz = "timestamptz(6)";
    }
}