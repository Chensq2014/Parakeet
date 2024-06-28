using System;
using System.Collections.Generic;

namespace Parakeet.Net.ROClient.Models
{
    public class DeviceAddedModel : ModelBase
    {
        public override string CommandName => "add_device";

        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设备序列号(原始编码，非转发编码)
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType Type { get; set; }

        /// <summary>
        /// 设备密钥
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 省，直辖市，自治区
        /// </summary>
        public string ParentArea { get; set; }

        /// <summary>
        /// 地级市，自治州
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 区县
        /// </summary>
        public string SubArea { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 供应商编码（持续更新中...）
        /// (重庆)科技有限公司             0001  采集设备
        /// 上海宇叶电子科技有限公司	          0061	塔吊、升降机
        /// 安徽聚正科技有限公司	              0062	塔吊、升降机、卸料台
        /// 四川成都九晨科技有限公司	          0063	实名制
        /// 安徽合肥川达信息科技有限责任公司	  0064	环境、塔吊、升降机、卸料台
        /// 浙江大华技术股份有限公司	          0065	视频
        /// 浙江海康威视数字技术股份有限公司      0066	视频
        /// 深圳市翼慧通科技有限公司	          0067	实名制
        /// 威海精讯畅通电子科技有限公司	      0068	环境
        /// 山东仁科测控技术有限公司	          0069	环境
        /// 四川成都泰测科技有限公司	          0070	结构、基坑、隧道、大坝
        /// 浙江杭州宇泛智能科技有限公司	      0071	实名制
        /// 深圳智必选(臻识)科技有限公司	      0072	人脸识别
        /// 杭州力知电子技术有限公司	          0073	爬架
        /// 深圳润德电子技术有限公司	          0074	人员定位
        /// 厦门俊煜信息科技有限公司	          0000	人脸识别
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 扩展
        /// </summary>
        public List<DeviceExtended> Extendeds { get; set; } = new List<DeviceExtended>();

        /// <summary>
        /// 密钥
        /// </summary>
        public List<DeviceKeySecret> KeySecrets { get; set; } = new List<DeviceKeySecret>();
    }
}