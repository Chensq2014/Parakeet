using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Values;

namespace Parakeet.Net.ValueObjects
{
    /// <summary>
    ///     地址(值类型地址)
    /// </summary>
    public class Address : ValueObject
    {
        /// <summary>
        ///     带参构造函数
        /// </summary>
        /// <param name="detail">详细地址 门牌号</param>
        /// <param name="village">村</param>
        /// <param name="street">街道</param>
        /// <param name="area">区/县</param>
        /// <param name="city">城市/自治区/特区/港澳台</param>
        /// <param name="province">省市/自治区/直辖市/特区/港澳台</param>
        /// <param name="country">国家</param>
        /// <param name="zipCode">邮政编码</param>
        /// <param name="code">位置区域代码 --过滤数据用</param>
        public Address(
            string detail = "",
            string village = "",
            string street = "",
            string area = "",
            string city = "",
            string province = "",
            string country = "",
            string zipCode = "",
            string code = "")
        {
            Detail = detail;
            Village = village;
            Street = street;
            Area = area;
            City = city;
            Province = province;
            Country = country;
            ZipCode = zipCode;
            Code = code;
        }

        /// <summary>
        ///     位置区域代码 Key 用于数据精准过滤
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string Code { get; set; }

        /// <summary>
        ///     邮编
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string ZipCode { get; set; }

        /// <summary>
        ///     国家
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128)]
        public string Country { get; set; }

        /// <summary>
        ///     省/直辖市/自治区
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128)]
        public string Province { get; set; }

        /// <summary>
        ///     城市/市/(特/自治)区
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128)]
        public string City { get; set; }

        /// <summary>
        ///     区/县
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength128)]
        public string Area { get; set; }

        /// <summary>
        ///     街道
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Street { get; set; }

        /// <summary>
        ///     村
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string Village { get; set; }

        /// <summary>
        ///     详细地址(小区-楼栋-门牌号)
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512)]
        public string Detail { get; set; }

        /// <summary>
        ///     完整地址 扩展字段
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512)]
        public string FullName { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Code;
            yield return Village;
            yield return Street;
            yield return Area;
            yield return City;
            yield return Province;
            yield return Country;
            yield return ZipCode;
            yield return Detail;
            yield return FullName;
        }

        /// <summary>
        ///     获取完整地址
        /// </summary>
        /// <returns></returns>
        public string GetAddressName()
        {
            return $"{FullName ?? Country + Province + City + Area + Street + Village + Detail}";
        }

        /// <summary>
        /// 更新Address
        /// </summary>
        /// <param name="address"></param>
        public void UpdateAddress(Address address)
        {
            Code = address.Code;
            Area = address.Area;
            City = address.City;
            Country = address.Country;
            Detail = address.Detail;
            FullName = address.FullName;
            Province = address.Province;
            Street = address.Street;
            Village = address.Village;
            ZipCode = address.ZipCode;
        }
    }
}