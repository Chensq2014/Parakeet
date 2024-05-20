using Common.Dtos;
using Common.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Parakeet.Net.SignalR;
using System.Linq;
using System.Threading.Tasks;
using Common.Enums;
using Common.Events;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Parakeet.Net.EventHandlers
{
    /// <summary>
    /// 用户消息 ILocalEventHandler/IDistributedEventHandler
    /// </summary>
    public class UserEventHandler : ILocalEventHandler<ReadNotifyEvent>, ITransientDependency
    {
        private readonly IRepository<Notify> _notifyRepository;
        private readonly IDistributedCache<NotifyCountOut> _cacheManager;
        private IHubContext<NotifyHub> _context;
        //public CurrentUser CurrentUser { get; set; }
        public IEventBus EventBus { get; set; }
        private static readonly object Lock = new object();
        public UserEventHandler(
            IRepository<Notify> notifyRepository,
            IDistributedCache<NotifyCountOut> cacheManager,
            IHubContext<NotifyHub> context)
        {
            _cacheManager = cacheManager;
            _notifyRepository = notifyRepository;
            _context = context;
            EventBus = NullDistributedEventBus.Instance;
        }

        /// <summary>
        /// 设置用户未读消息数
        /// </summary>
        /// <param name="eventData">用户未读消息事件</param>
        /// <returns>用户未读消息数</returns>
        [UnitOfWork]
        public async Task HandleEventAsync(ReadNotifyEvent eventData)
        {
            //这个地方替换为缓存数据自增或自减
            var key = $"{eventData.ToUserId}_Notify";

            #region 使用缓存

            var userNotifies = await _cacheManager
                .GetOrAddAsync(key, async () =>
                {
                    var notifies = await (await _notifyRepository.GetQueryableAsync()).AsNoTracking()
                        .Where(m => m.ToUserId == eventData.ToUserId && !m.IsRead).ToListAsync();
                    return new NotifyCountOut
                    {
                        SystemNotifyCount = notifies.Count(m => m.NotifyType == NotifyType.系统消息),
                        ApplicationNotifyCount = notifies.Count(m => m.NotifyType == NotifyType.应用消息)
                    };
                });

            lock (Lock)
            {
                if (eventData.NotifyType == NotifyType.系统消息)
                {
                    userNotifies.SystemNotifyCount += eventData.NotifyCount;
                    userNotifies.SystemNotifyCount = userNotifies.SystemNotifyCount >= 0 ? userNotifies.SystemNotifyCount : 0;
                }
                else
                {
                    userNotifies.ApplicationNotifyCount += eventData.NotifyCount;
                    userNotifies.ApplicationNotifyCount =
                        userNotifies.ApplicationNotifyCount >= 0
                            ? userNotifies.ApplicationNotifyCount
                            : 0;
                }
                _cacheManager.Set(key, userNotifies);
            }

            #endregion

            #region 使用redis Api no pass

            //using (RedisHashService service = new RedisHashService())
            //{
            //    string countTypeField = eventData.NotifyType == NotifyType.系统消息
            //        ? "SystemNotifyCount"
            //        : "ApplicationNotifyCount";
            //    var isExist = service.HashExists($"{NetConsts.UserUnReadNotify}_{key}", countTypeField);
            //    if (isExist)
            //    {
            //        service.HashIncrement($"{NetConsts.UserUnReadNotify}_{key}", countTypeField, eventData.NotifyCount);
            //    }
            //    else
            //    {
            //        var notifies = await _notifyRepository.GetAll().Where(m => m.ToUserId == AbpSession.UserId && !m.IsRead).ToListAsync();
            //        service.HashSet($"{NetConsts.UserUnReadNotify}_{key}", countTypeField, notifies.Count(m => m.NotifyType == eventData.NotifyType));
            //    }
            //}

            #endregion

            #region SignalR

            //禁用signalr,对实时性要求不高
            await _context.Clients.All.SendCoreAsync("Receive", new object[] { userNotifies });

            #endregion
        }
    }
}
