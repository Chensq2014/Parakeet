using System.Collections.Generic;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// 存储所有缓存Key的缓存键设置类
    /// </summary>
    public class CacheKeys
    {
        /// <summary>
        /// 所有缓存Key集合
        /// </summary>
        public static List<string> Keys { get; set; } = new List<string>();

        /// <summary>
        /// 日志消息先后顺序计数
        /// </summary>
        public static int LogCount { get; set; } = 0;

        static CacheKeys()
        {
            Keys.Add(DeviceDevicePoolSerialNo);
            Keys.Add(DeviceDevicePoolFakeNo);
            Keys.Add(AreaAreaPoolCodeLevel);
            Keys.Add(KeySecretKeySecretPoolAreaSerialNo);
            Keys.Add(DevicePacketHandlerPoolHeader);
            Keys.Add(DeviceSerialNo);
            Keys.Add(CraneBasicSerialNoCraneId);
            Keys.Add(WorkerDeviceUserIdSerialNoPersonnelId);
            Keys.Add(CraneCalibrationSerialNo);
            Keys.Add(TicketAppKey);
            Keys.Add(HeartbeatDeviceArea);
            Keys.Add(HeartbeatArea);
            Keys.Add(LicenseAppIdAppKey);
            Keys.Add(LicenseId);
            Keys.Add(LicenseName);
        }

        public const string DeviceDevicePoolSerialNo = "Device:DevicePool:SerialNo:{0}";

        public const string DeviceDevicePoolFakeNo = "Device:DevicePool:FakeNo:{0}";

        public const string AreaAreaPoolCodeLevel = "Area:AreaPool:Code-level:{0}-{1}";

        public const string KeySecretKeySecretPoolAreaSerialNo = "KeySecret:KeySecretPool:Area-SerialNo:{0}-{1}";

        public const string DevicePacketHandlerPoolHeader = "Device:PacketHandlerPool:Header:{0}";

        public const string DeviceSerialNo = "Device:SerialNo:{0}";

        public const string DeviceForwardRecord = "Device:Forward:Record";//redis数据常量

        public const string DeviceKeySecretId = "Device:KeySecretId:{0}";

        public const string DeviceKeySecretAreaFakeNo = "Device:KeySecret:DeviceId:{0}";

        public const string DeviceKeySecretAreaDeviceId = "Device:KeySecret:Area:{0}:DeviceId:{1}";

        public const string CraneBasicSerialNoCraneId = "CraneBasic:SerialNo:{0}:CraneId:{1}";

        public const string FPTerminalBasicSerialNoSensorTerminalIdPointNumber = "Foundationpit:SerialNo:{0}:Sensor{1}:TerminalId:{2}:PointNumber:{3}";

        public const string FPTerminalBasicFakeNoSensorTerminalIdPointNumber = "Foundationpit:FakeNo:{0}:Sensor{1}:TerminalId:{2}:PointNumber:{3}";

        public const string WorkerDeviceUserIdSerialNoPersonnelId = "WorkerDevice:UserId:SerialNo:{0}:PersonnelId:{1}";

        public const string CraneCalibrationSerialNo = "CraneCalibration:SerialNo:{0}";

        public const string TicketAppKey = "Ticket:AppId:{0}:AppKey:{1}";

        public const string HeartbeatDeviceArea = "Heartbeat:Device:Area:{0}";

        public const string HeartbeatArea = "Heartbeat:Area";

        public const string AreaTenantPoolCode = "AreaTenant:Pool:Code:{0}";

        public const string LicenseAppIdAppKey = "License:AppId:{0}:AppKey:{1}";
        public const string LicenseId = "License:Id:{0}";
        public const string LicenseName = "License:Name:{0}";
    }
}
