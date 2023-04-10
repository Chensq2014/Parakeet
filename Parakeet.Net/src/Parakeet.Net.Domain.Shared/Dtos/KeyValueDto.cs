using System.ComponentModel;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    ///     包含Key Value的Dto
    /// </summary>
    public class KeyValueDto<TKey, TValue>
    {
        /// <summary>
        /// KeyValueDto
        /// </summary>
        public KeyValueDto()
        {
        }

        /// <summary>
        /// KeyValueDto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public KeyValueDto(TKey id, TValue value)
        {
            Id = id;
            Text = id?.ToString();
            Value = value;
        }

        /// <summary>
        ///     唯一标识Id
        /// </summary>
        [Description("唯一标识Id")]
        public TKey Id { get; set; }

        /// <summary>
        ///     Text
        /// </summary>
        [Description("文本")]
        public string Text { get; set; }

        /// <summary>
        ///     Value
        /// </summary>
        [Description("值")]
        public TValue Value { get; set; }

        /// <summary>
        ///     ExtraValue
        /// </summary>
        [Description("扩展值")]
        public TValue ExtraValue { get; set; }
    }

    public class KeyValueDto : KeyValueDto<int, string>
    {
        public KeyValueDto(int key, string value) : base(key, value)
        {
        }
    }
}