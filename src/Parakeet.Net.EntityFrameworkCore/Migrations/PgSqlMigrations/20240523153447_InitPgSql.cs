using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parakeet.Net.Migrations.PgSqlMigrations
{
    /// <inheritdoc />
    public partial class InitPgSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "device");

            migrationBuilder.CreateTable(
                name: "d_CraneAlarm",
                schema: "device",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<short>(type: "smallint", nullable: false),
                    AlarmType = table.Column<short>(type: "smallint", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    WarnLevel = table.Column<int>(type: "integer", nullable: false),
                    WarnType = table.Column<int>(type: "integer", nullable: false),
                    WindSpeedPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    TiltPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    BrakingState = table.Column<int>(type: "integer", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CraneId = table.Column<short>(type: "smallint", nullable: false),
                    Fall = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<decimal>(type: "numeric", nullable: true),
                    Range = table.Column<decimal>(type: "numeric", nullable: true),
                    Rotation = table.Column<decimal>(type: "numeric", nullable: true),
                    Load = table.Column<decimal>(type: "numeric", nullable: true),
                    WeightPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    WindSpeed = table.Column<decimal>(type: "numeric", nullable: true),
                    TiltAngle = table.Column<decimal>(type: "numeric", nullable: true),
                    Torque = table.Column<decimal>(type: "numeric", nullable: true),
                    TorquePercent = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_d_CraneAlarm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "d_CraneBasic",
                schema: "device",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CraneType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ArticulatedLength = table.Column<decimal>(type: "numeric", nullable: true),
                    CraneName = table.Column<string>(type: "text", nullable: true),
                    MinLoadWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    MinTorque = table.Column<decimal>(type: "numeric", nullable: true),
                    MinAngle = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxAngle = table.Column<decimal>(type: "numeric", nullable: true),
                    MinRadius = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxRadius = table.Column<decimal>(type: "numeric", nullable: true),
                    MinHeight = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxHeight = table.Column<decimal>(type: "numeric", nullable: true),
                    MinWindSpeed = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxWindSpeed = table.Column<decimal>(type: "numeric", nullable: true),
                    LimitValue = table.Column<decimal>(type: "numeric", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CraneId = table.Column<short>(type: "smallint", nullable: false),
                    ShortArm = table.Column<decimal>(type: "numeric", nullable: true),
                    LongArm = table.Column<decimal>(type: "numeric", nullable: true),
                    TowerHatHeight = table.Column<decimal>(type: "numeric", nullable: true),
                    BoomHeight = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxLoadWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxTorque = table.Column<decimal>(type: "numeric", nullable: true),
                    Fall = table.Column<int>(type: "integer", nullable: true),
                    HookWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    CompassAngle = table.Column<decimal>(type: "numeric", nullable: true),
                    X = table.Column<decimal>(type: "numeric", nullable: true),
                    Y = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_d_CraneBasic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "d_CraneRecord",
                schema: "device",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceId = table.Column<string>(type: "text", nullable: true),
                    SafeLoad = table.Column<decimal>(type: "numeric", nullable: true),
                    Obliquity = table.Column<decimal>(type: "numeric", nullable: true),
                    DirAngle = table.Column<decimal>(type: "numeric", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    IdCard = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    DriverName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PowerStatus = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    TiltX = table.Column<decimal>(type: "numeric", nullable: true),
                    TiltY = table.Column<decimal>(type: "numeric", nullable: true),
                    AlarmCode = table.Column<int>(type: "integer", nullable: true),
                    LoadWarnState = table.Column<int>(type: "integer", nullable: false),
                    KnWarnState = table.Column<int>(type: "integer", nullable: false),
                    AngleWarnState = table.Column<int>(type: "integer", nullable: false),
                    RadiusWarnState = table.Column<int>(type: "integer", nullable: false),
                    HeightWarnState = table.Column<int>(type: "integer", nullable: false),
                    WindSpeedWarnState = table.Column<int>(type: "integer", nullable: false),
                    RotationWarnState = table.Column<int>(type: "integer", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CraneId = table.Column<short>(type: "smallint", nullable: false),
                    Fall = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<decimal>(type: "numeric", nullable: true),
                    Range = table.Column<decimal>(type: "numeric", nullable: true),
                    Rotation = table.Column<decimal>(type: "numeric", nullable: true),
                    Load = table.Column<decimal>(type: "numeric", nullable: true),
                    WeightPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    WindSpeed = table.Column<decimal>(type: "numeric", nullable: true),
                    TiltAngle = table.Column<decimal>(type: "numeric", nullable: true),
                    Torque = table.Column<decimal>(type: "numeric", nullable: true),
                    TorquePercent = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_d_CraneRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "d_EnvironmentRecord",
                schema: "device",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    COFlag = table.Column<string>(type: "text", nullable: true),
                    SO2Flag = table.Column<string>(type: "text", nullable: true),
                    NO2Flag = table.Column<string>(type: "text", nullable: true),
                    O3Flag = table.Column<string>(type: "text", nullable: true),
                    VisibilityFlag = table.Column<string>(type: "text", nullable: true),
                    TVOCFlag = table.Column<string>(type: "text", nullable: true),
                    TSPFlag = table.Column<string>(type: "text", nullable: true),
                    PM2P5Flag = table.Column<string>(type: "text", nullable: true),
                    PM10Flag = table.Column<string>(type: "text", nullable: true),
                    NoiseFlag = table.Column<string>(type: "text", nullable: true),
                    WindDirectionFlag = table.Column<string>(type: "text", nullable: true),
                    WindSpeedFlag = table.Column<string>(type: "text", nullable: true),
                    TemperatureFlag = table.Column<string>(type: "text", nullable: true),
                    HumidityFlag = table.Column<string>(type: "text", nullable: true),
                    PressureFlag = table.Column<string>(type: "text", nullable: true),
                    RainfallFlag = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PM2P5 = table.Column<decimal>(type: "numeric", nullable: true),
                    PM10 = table.Column<decimal>(type: "numeric", nullable: true),
                    Temperature = table.Column<decimal>(type: "numeric", nullable: true),
                    Noise = table.Column<decimal>(type: "numeric", nullable: true),
                    Humidity = table.Column<decimal>(type: "numeric", nullable: true),
                    WindDirection = table.Column<decimal>(type: "numeric", nullable: true),
                    WindSpeed = table.Column<decimal>(type: "numeric", nullable: true),
                    Rainfall = table.Column<decimal>(type: "numeric", nullable: true),
                    Pressure = table.Column<decimal>(type: "numeric", nullable: true),
                    CO = table.Column<decimal>(type: "numeric", nullable: true),
                    SO2 = table.Column<decimal>(type: "numeric", nullable: true),
                    NO2 = table.Column<decimal>(type: "numeric", nullable: true),
                    O3 = table.Column<decimal>(type: "numeric", nullable: true),
                    Visibility = table.Column<decimal>(type: "numeric", nullable: true),
                    TVOC = table.Column<decimal>(type: "numeric", nullable: true),
                    AQI = table.Column<decimal>(type: "numeric", nullable: true),
                    TSP = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_d_EnvironmentRecord", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "d_CraneAlarm",
                schema: "device");

            migrationBuilder.DropTable(
                name: "d_CraneBasic",
                schema: "device");

            migrationBuilder.DropTable(
                name: "d_CraneRecord",
                schema: "device");

            migrationBuilder.DropTable(
                name: "d_EnvironmentRecord",
                schema: "device");
        }
    }
}
