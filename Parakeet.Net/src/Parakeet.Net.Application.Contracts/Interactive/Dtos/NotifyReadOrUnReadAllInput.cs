using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Interactive.Dtos
{
    public class NotifyReadOrUnReadAllInput : NotifyDeleteAllInput
    {
        /// <summary>
        /// 标记为已读/未读
        /// </summary>
        [Required]
        public bool ReadAll { get; set; }
    }
}
