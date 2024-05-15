//using AutoMapper.QueryableExtensions;
//using Microsoft.EntityFrameworkCore;
//using Common.Dtos;
//using Common.Entities;
//using Common.Interfaces;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Parakeet.Net.Needs
//{
//    /// <summary>
//    /// 需求附件服务
//    /// </summary>
//    public class NeedAttachmentAppService : BaseParakeetAppService<NeedAttachment>, INeedAttachmentAppService
//    {
//        //private readonly IParakeetRepository<NeedAttachment> _needAttachmentRepository;
//        public NeedAttachmentAppService(
//            IParakeetRepository<NeedAttachment> baseRepository) : base(baseRepository)
//        {
//            //_needAttachmentRepository = baseRepository;
//        }

//        /// <summary>
//        /// 获取所有附件信息
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<IList<NeedAttachmentDto>> GetNeedAttachments(InputIdDto input)
//        {
//            return await (await Repository.GetQueryableAsync())
//                .Where(m => m.NeedId == input.Id)
//                .OrderByDescending(n => n.Order)
//                .ProjectTo<NeedAttachmentDto>(Configuration)
//                .ToListAsync();
//        }
//    }
//}
