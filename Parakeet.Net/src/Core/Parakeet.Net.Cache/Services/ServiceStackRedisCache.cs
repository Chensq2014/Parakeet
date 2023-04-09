using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Parakeet.Net.Dtos;
using Parakeet.Net.Interfaces;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Text;
using System.Collections;
using System.Diagnostics;

namespace Parakeet.Net.Cache
{
    /// <summary>
    /// ServiceStack.Redis CacheClient 可以重新封装实现IDistributedCache接口
    /// </summary>
    public class ServiceStackRedisCache : IDistributedCache, IHandlerType, IDisposable
    {
        #region 基础字段属性

        /// <summary>
        /// ServiceStack
        /// </summary>
        public string HandlerType => "ServiceStack";

        /// <summary>
        /// 连接字符串配置
        /// </summary>
        private readonly RedisConnOptions _redisOptions;

        /// <summary>
        /// Redis客户端client
        /// </summary>
        private readonly IRedisClient _client;

        #endregion

        #region 构造 释放

        /// <summary>
        /// 构造函数 依赖注入redisOption
        /// </summary>
        /// <param name="redisOption"></param>
        public ServiceStackRedisCache(IOptionsMonitor<RedisConnOptions> redisOption)
        {
            this._redisOptions = redisOption.CurrentValue;
            
            //ServiceStack 暂还不支持.Net7 
            //_client = new RedisClient(_redisOptions.Host, _redisOptions.Port, null, _redisOptions.DatabaseId);
        }


        /// <summary>
        /// 释放client
        /// </summary>
        public void Dispose()
        {
            this.TryCatchException(delegate
            {
                this._client.Dispose();
            }, string.Empty);
        }
        #endregion

        #region 获取client 更改db
        /// <summary>
        /// 获取client
        /// </summary>
        /// <returns></returns>
        public IRedisClient GetClient()
        {
            return _client;
        }

        /// <summary>
        /// 获取更换database后的client
        /// </summary>
        /// <returns></returns>
        public IRedisClient ChangeDb(int dbId)
        {
            ((RedisClient)_client).ChangeDb(dbId);
            return _client;
        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        private void TryCatchException(Action action, string key)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                //Logger.None.Debug($"{ex.Message}");
            }
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private T TryCatch<T>(Func<T> action, string key)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Exception ex = null;
            bool isError = false;
            T result;
            try
            {
                result = action();
            }
            catch (Exception exinfo)
            {
                isError = true;
                throw exinfo;
                ex = exinfo;
            }
            finally
            {

                sw.Stop();

            }
            return result;
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        private void TryCatch(Action action, string key)
        {
            Stopwatch sw = Stopwatch.StartNew();
            bool isError = false;
            Exception ex = null;
            try
            {
                action();
            }
            catch (Exception exinfo)
            {
                isError = true;
                throw exinfo;
            }
            finally
            {
                sw.Stop();

            }
        }

        #endregion

        #region 添加泛型缓存
        /// <summary>
        /// 添加泛型缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add<T>(string key, T value)
        {
            return this.TryCatch<bool>(() => this._client.Add<T>(key, value), key);
        }

        /// <summary>
        /// 添加泛型缓存带过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            return this.TryCatch<bool>(() => this._client.Add<T>(key, value, expiresAt), key);
        }

        /// <summary>
        /// 添加泛型缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            return this.TryCatch<bool>(() => this._client.Add<T>(key, value, expiresIn), key);
        }

        #endregion

        #region 自增自减

        /// <summary>
        /// 自增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public long Increment(string key, uint amount)
        {
            return this.TryCatch<long>(() => this._client.Increment(key, amount), key);
        }

        /// <summary>
        /// 自减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public long Decrement(string key, uint amount)
        {
            return this.TryCatch<long>(() => this._client.Decrement(key, amount), key);
        }

        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long DecrementValue(string key)
        {
            return this.TryCatch<long>(() => this._client.DecrementValue(key), key);
        }

        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public long DecrementValueBy(string key, int count)
        {
            return this.TryCatch<long>(() => this._client.DecrementValueBy(key, count), key);
        }


        public double IncrementItemInSortedSet(string setId, string value, double incrementBy)
        {
            return this.TryCatch<double>(() => this._client.IncrementItemInSortedSet(setId, value, incrementBy), setId);
        }

        public double IncrementItemInSortedSet(string setId, string value, long incrementBy)
        {
            return this.TryCatch<double>(() => this._client.IncrementItemInSortedSet(setId, value, incrementBy), setId);
        }

        public long IncrementValue(string key)
        {
            return this.TryCatch<long>(() => this._client.IncrementValue(key), key);
        }

        public long IncrementValueBy(string key, int count)
        {
            return this.TryCatch<long>(() => this._client.IncrementValueBy(key, count), key);
        }

        public long IncrementValueInHash(string hashId, string key, int incrementBy)
        {
            return this.TryCatch<long>(() => this._client.IncrementValueInHash(hashId, key, incrementBy), hashId);
        }

        #endregion

        #region 获取泛型缓存

        /// <summary>
        /// 获取泛型缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return this.TryCatch<T>(() => this._client.Get<T>(key), key);
        }

        /// <summary>
        /// 获取所有泛型缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            return this.TryCatch<IDictionary<string, T>>(() => this._client.GetAll<T>(keys), keys.FirstOrDefault<string>());
        }

        #endregion

        #region 清空 移除

        /// <summary>
        /// 清空
        /// </summary>
        public void FlushAll()
        {
            this.TryCatch(delegate
            {
                this._client.FlushAll();
            }, string.Empty);
        }


        /// <summary>
        /// 根据key移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return this.TryCatch<bool>(() => this._client.Remove(key), key);
        }

        /// <summary>
        /// 移除所有keys
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveAll(IEnumerable<string> keys)
        {
            this.TryCatch(delegate
            {
                this._client.RemoveAll(keys);
            }, keys.FirstOrDefault<string>());
        }



        #endregion

        #region 替换

        /// <summary>
        /// 替换key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Replace<T>(string key, T value)
        {
            return this.TryCatch<bool>(() => this._client.Replace<T>(key, value), key);
        }

        /// <summary>
        /// 替换key 带过期时间DateTime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            return this.TryCatch<bool>(() => this._client.Replace<T>(key, value, expiresAt), key);
        }

        /// <summary>
        /// 替换key TimeSpan
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return this.TryCatch<bool>(() => this._client.Replace<T>(key, value, expiresIn), key);
        }


        #endregion

        #region 设置缓存

        /// <summary>
        /// 设置泛型缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public bool Set<T>(string key, T value)
        {
            return this.TryCatch<bool>(() => this._client.Set<T>(key, value), key);
        }

        /// <summary>
        /// 设置泛型缓存带过期时间DateTime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return this.TryCatch<bool>(() => this._client.Set<T>(key, value, expiresAt), key);
        }

        /// <summary>
        /// 设置泛型缓存带 TimeSpan
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return this.TryCatch<bool>(() => this._client.Set<T>(key, value, expiresIn), key);
        }

        /// <summary>
        /// 根据字典设置泛型缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public void SetAll<T>(IDictionary<string, T> values)
        {
            this.TryCatch(delegate
            {
                this._client.SetAll<T>(values);
            }, values.Keys.FirstOrDefault<string>());
        }


        #endregion

        #region 根据对象信息删除缓存

        /// <summary>
        /// 根据泛型删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Delete<T>(T entity) where T : class, new()
        {
            this.TryCatch(delegate
            {
                this._client.Delete<T>(entity);
            }, string.Empty);
        }

        /// <summary>
        /// 根据泛型删除所有
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public void DeleteAll<TEntity>() where TEntity : class, new()
        {
            this.TryCatch(delegate
            {
                this._client.DeleteAll<TEntity>();
            }, string.Empty);
        }

        /// <summary>
        /// 根据对象Id删除缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public void DeleteById<T>(object id) where T : class, new()
        {
            this.TryCatch(delegate
            {
                this._client.DeleteById<T>(id);
            }, string.Empty);
        }

        /// <summary>
        /// 根据对象Ids删除缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        public void DeleteByIds<T>(ICollection ids) where T : class, new()
        {
            this.TryCatch(delegate
            {
                this._client.DeleteById<T>(ids);
            }, string.Empty);
        }

        #endregion

        #region 根据对象信息获取缓存

        /// <summary>
        /// 根据对象Id信息获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(object id) where T : class, new()
        {
            return this.TryCatch<T>(() => this._client.GetById<T>(id), string.Empty);
        }

        /// <summary>
        /// 根据对象Ids信息获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<T> GetByIds<T>(ICollection ids) where T : class, new()
        {
            return this.TryCatch<IList<T>>(() => this._client.GetByIds<T>(ids), string.Empty);
        }

        #endregion

        #region Store 存储对象

        /// <summary>
        /// Store 泛型存储对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Store<T>(T entity) where T : class, new()
        {
            return this.TryCatch<T>(() => this._client.Store<T>(entity), string.Empty);
        }

        /// <summary>
        /// Store 存储所有对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void StoreAll<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, new()
        {
            this.TryCatch(delegate
            {
                this._client.StoreAll<TEntity>(entities);
            }, string.Empty);
        }


        #endregion

        #region List

        /// <summary>
        /// 添加到list缓存
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="value"></param>
        public void AddItemToList(string listId, string value)
        {
            this.TryCatch(delegate
            {
                this._client.AddItemToList(listId, value);
            }, listId);
        }

        /// <summary>
        /// 添加范围到list
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="values"></param>
        public void AddRangeToList(string listId, List<string> values)
        {
            this.TryCatch(delegate
            {
                this._client.AddRangeToList(listId, values);
            }, listId);
        }

        /// <summary>
        /// 添加到BlockingDequeueItemFromList
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public string BlockingDequeueItemFromList(string listId, TimeSpan? timeOut)
        {
            return this.TryCatch<string>(() => this._client.BlockingDequeueItemFromList(listId, timeOut), listId);
        }

        public KeyValuePair<string, string> BlockingDequeueItemFromLists(string[] listIds, TimeSpan? timeOut)
        {
            return this.TryCatch<KeyValuePair<string, string>>(delegate
            {
                ItemRef item = this._client.BlockingDequeueItemFromLists(listIds, timeOut);
                return new KeyValuePair<string, string>(item.Id, item.Item);
            }, listIds[0]);
        }

        public string BlockingPopAndPushItemBetweenLists(string fromListId, string toListId, TimeSpan? timeOut)
        {
            return this.TryCatch<string>(() => this._client.BlockingPopAndPushItemBetweenLists(fromListId, toListId, timeOut), fromListId);
        }

        public string BlockingPopItemFromList(string listId, TimeSpan? timeOut)
        {
            return this.TryCatch<string>(() => this._client.BlockingPopItemFromList(listId, timeOut), listId);
        }

        public KeyValuePair<string, string> BlockingPopItemFromLists(string[] listIds, TimeSpan? timeOut)
        {
            return this.TryCatch<KeyValuePair<string, string>>(delegate
            {
                ItemRef item = this._client.BlockingPopItemFromLists(listIds, timeOut);
                return new KeyValuePair<string, string>(item.Id, item.Item);
            }, listIds[0]);
        }

        public string BlockingRemoveStartFromList(string listId, TimeSpan? timeOut)
        {
            return this.TryCatch<string>(() => this._client.BlockingRemoveStartFromList(listId, timeOut), listId);
        }

        public KeyValuePair<string, string> BlockingRemoveStartFromLists(string[] listIds, TimeSpan? timeOut)
        {
            return this.TryCatch<KeyValuePair<string, string>>(delegate
            {
                ItemRef item = this._client.BlockingRemoveStartFromLists(listIds, timeOut);
                return new KeyValuePair<string, string>(item.Id, item.Item);
            }, listIds[0]);
        }

        /// <summary>
        /// 从list出队
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public string DequeueItemFromList(string listId)
        {
            return this.TryCatch<string>(() => this._client.DequeueItemFromList(listId), listId);
        }

        /// <summary>
        /// 入队list 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="value"></param>
        public void EnqueueItemOnList(string listId, string value)
        {
            this.TryCatch(delegate
            {
                this._client.EnqueueItemOnList(listId, value);
            }, listId);
        }

        public List<string> GetAllItemsFromList(string listId)
        {
            return this.TryCatch<List<string>>(() => this._client.GetAllItemsFromList(listId), listId);
        }

        public string PopAndPushItemBetweenLists(string fromListId, string toListId)
        {
            return this.TryCatch<string>(() => this._client.PopAndPushItemBetweenLists(fromListId, toListId), fromListId);
        }

        public string PopItemFromList(string listId)
        {
            return this.TryCatch<string>(() => this._client.PopItemFromList(listId), listId);
        }


        public void PrependItemToList(string listId, string value)
        {
            this.TryCatch(delegate
            {
                this._client.PrependItemToList(listId, value);
            }, listId);
        }

        public void PrependRangeToList(string listId, List<string> values)
        {
            this.TryCatch(delegate
            {
                this._client.PrependRangeToList(listId, values);
            }, listId);
        }

        public void PushItemToList(string listId, string value)
        {
            this.TryCatch(delegate
            {
                this._client.PushItemToList(listId, value);
            }, listId);
        }

        public void RemoveAllFromList(string listId)
        {
            this.TryCatch(delegate
            {
                this._client.Remove(listId);
            }, listId);
        }

        public string RemoveEndFromList(string listId)
        {
            return this.TryCatch<string>(() => this._client.RemoveEndFromList(listId), listId);
        }

        #endregion

        #region Set

        /// <summary>
        /// 添加到Set列表缓存
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="item"></param>
        public void AddItemToSet(string setId, string item)
        {
            this.TryCatch(delegate
            {
                this._client.AddItemToSet(setId, item);
            }, setId);
        }

        /// <summary>
        /// 添加范围到set
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="items"></param>
        public void AddRangeToSet(string setId, List<string> items)
        {
            this.TryCatch(delegate
            {
                this._client.AddRangeToSet(setId, items);
            }, setId);
        }

        public HashSet<string> GetAllItemsFromSet(string setId)
        {
            return this.TryCatch<HashSet<string>>(() => this._client.GetAllItemsFromSet(setId), setId);
        }


        public void MoveBetweenSets(string fromSetId, string toSetId, string item)
        {
            this.TryCatch(delegate
            {
                this._client.MoveBetweenSets(fromSetId, toSetId, item);
            }, fromSetId);
        }
        public string PopItemFromSet(string setId)
        {
            return this.TryCatch<string>(() => this._client.PopItemFromSet(setId), setId);
        }

        #endregion

        #region ZSet


        /// <summary>
        /// 添加到SortedSet
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddItemToSortedSet(string setId, string value)
        {
            return this.TryCatch<bool>(() => this._client.AddItemToSortedSet(setId, value), setId);
        }

        /// <summary>
        /// 添加到SortedSet with score
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool AddItemToSortedSet(string setId, string value, double score)
        {
            return this.TryCatch<bool>(() => this._client.AddItemToSortedSet(setId, value, score), setId);
        }

        /// <summary>
        /// 添加范围到SortedSet
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="values"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool AddRangeToSortedSet(string setId, List<string> values, double score)
        {
            return this.TryCatch<bool>(() => this._client.AddRangeToSortedSet(setId, values, score), setId);
        }

        /// <summary>
        /// 添加范围到SortedSet with score
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="values"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool AddRangeToSortedSet(string setId, List<string> values, long score)
        {
            return this.TryCatch<bool>(() => this._client.AddRangeToSortedSet(setId, values, score), setId);
        }

        public List<string> GetAllItemsFromSortedSet(string setId)
        {
            return this.TryCatch<List<string>>(() => this._client.GetAllItemsFromSortedSet(setId), setId);
        }

        public List<string> GetAllItemsFromSortedSetDesc(string setId)
        {
            return this.TryCatch<List<string>>(() => this._client.GetAllItemsFromSortedSetDesc(setId), setId);
        }

        public IDictionary<string, double> GetAllWithScoresFromSortedSet(string setId)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetAllWithScoresFromSortedSet(setId), setId);
        }

        public string PopItemWithHighestScoreFromSortedSet(string setId)
        {
            return this.TryCatch<string>(() => this._client.PopItemWithHighestScoreFromSortedSet(setId), setId);
        }

        public string PopItemWithLowestScoreFromSortedSet(string setId)
        {
            return this.TryCatch<string>(() => this._client.PopItemWithLowestScoreFromSortedSet(setId), setId);
        }

        #endregion

        #region String

        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long AppendToValue(string key, string value)
        {
            return this.TryCatch<long>(() => this._client.AppendToValue(key, value), key);
        }

        #endregion

        #region 判断Contains

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return this.TryCatch<bool>(() => this._client.ContainsKey(key), key);
        }

        public bool HashContainsEntry(string hashId, string key)
        {
            return this.TryCatch<bool>(() => this._client.HashContainsEntry(hashId, key), hashId);
        }

        #endregion

        #region 单独设置过期
        public bool ExpireEntryAt(string key, DateTime expireAt)
        {
            return this.TryCatch<bool>(() => this._client.ExpireEntryAt(key, expireAt), key);
        }

        public bool ExpireEntryIn(string key, TimeSpan expireIn)
        {
            return this.TryCatch<bool>(() => this._client.ExpireEntryIn(key, expireIn), key);
        }

        #endregion

        #region Hash
        public Dictionary<string, string> GetAllEntriesFromHash(string hashId)
        {
            return this.TryCatch<Dictionary<string, string>>(() => this._client.GetAllEntriesFromHash(hashId), hashId);
        }

        public HashSet<string> GetDifferencesFromSet(string fromSetId, params string[] withSetIds)
        {
            return this.TryCatch<HashSet<string>>(() => this._client.GetDifferencesFromSet(fromSetId, withSetIds), fromSetId);
        }

        #endregion

        #region 各种获取
        public List<string> GetAllKeys()
        {
            return this.TryCatch<List<string>>(() => this._client.GetAllKeys(), string.Empty);
        }


        public string GetAndSetEntry(string key, string value)
        {
            return this.TryCatch<string>(() => this._client.GetAndSetValue(key, value), key);
        }

        public T GetFromHash<T>(object id)
        {
            return this.TryCatch<T>(() => this._client.GetFromHash<T>(id), string.Empty);
        }

        public long GetHashCount(string hashId)
        {
            return this.TryCatch<long>(() => this._client.GetHashCount(hashId), hashId);
        }

        public List<string> GetHashKeys(string hashId)
        {
            return this.TryCatch<List<string>>(() => this._client.GetHashKeys(hashId), hashId);
        }

        public List<string> GetHashValues(string hashId)
        {
            return this.TryCatch<List<string>>(() => this._client.GetHashValues(hashId), hashId);
        }

        public HashSet<string> GetIntersectFromSets(params string[] setIds)
        {
            return this.TryCatch<HashSet<string>>(() => this._client.GetIntersectFromSets(setIds), setIds[0]);
        }

        public string GetItemFromList(string listId, int listIndex)
        {
            return this.TryCatch<string>(() => this._client.GetItemFromList(listId, listIndex), listId);
        }

        public long GetItemIndexInSortedSet(string setId, string value)
        {
            return this.TryCatch<long>(() => this._client.GetItemIndexInSortedSet(setId, value), setId);
        }

        public long GetItemIndexInSortedSetDesc(string setId, string value)
        {
            return this.TryCatch<long>(() => this._client.GetItemIndexInSortedSetDesc(setId, value), setId);
        }

        public double GetItemScoreInSortedSet(string setId, string value)
        {
            return this.TryCatch<double>(() => this._client.GetItemScoreInSortedSet(setId, value), setId);
        }

        public long GetListCount(string listId)
        {
            return this.TryCatch<long>(() => this._client.GetListCount(listId), listId);
        }

        public string GetRandomItemFromSet(string setId)
        {
            return this.TryCatch<string>(() => this._client.GetRandomItemFromSet(setId), setId);
        }

        public List<string> GetRangeFromList(string listId, int startingFrom, int endingAt)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromList(listId, startingFrom, endingAt), listId);
        }

        public List<string> GetRangeFromSortedList(string listId, int startingFrom, int endingAt)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedList(listId, startingFrom, endingAt), listId);
        }

        public List<string> GetRangeFromSortedSet(string setId, int fromRank, int toRank)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSet(setId, fromRank, toRank), setId);
        }

        public List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore), setId);
        }

        public List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore), setId);
        }

        public List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByHighestScore(setId, fromStringScore, toStringScore), setId);
        }

        public List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByHighestScore(setId, fromStringScore, toStringScore, skip, take), setId);
        }

        public List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore), setId);
        }

        public List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore), setId);
        }

        public List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByLowestScore(setId, fromStringScore, toStringScore), setId);
        }

        public List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetByLowestScore(setId, fromStringScore, toStringScore, skip, take), setId);
        }

        public List<string> GetRangeFromSortedSetDesc(string setId, int fromRank, int toRank)
        {
            return this.TryCatch<List<string>>(() => this._client.GetRangeFromSortedSetDesc(setId, fromRank, toRank), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSet(string setId, int fromRank, int toRank)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSet(setId, fromRank, toRank), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromStringScore, toStringScore), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromStringScore, toStringScore, skip, take), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromStringScore, toStringScore), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromStringScore, toStringScore, skip, take), setId);
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetDesc(string setId, int fromRank, int toRank)
        {
            return this.TryCatch<IDictionary<string, double>>(() => this._client.GetRangeWithScoresFromSortedSetDesc(setId, fromRank, toRank), setId);
        }

        public long GetSetCount(string setId)
        {
            return this.TryCatch<long>(() => this._client.GetSetCount(setId), setId);
        }

        public List<string> GetSortedEntryValues(string key, int startingFrom, int endingAt)
        {
            return this.TryCatch<List<string>>(() => this._client.GetSortedEntryValues(key, startingFrom, endingAt), key);
        }

        public long GetSortedSetCount(string setId)
        {
            return this.TryCatch<long>(() => this._client.GetSortedSetCount(setId), setId);
        }

        public long GetSortedSetCount(string setId, double fromScore, double toScore)
        {
            return this.TryCatch<long>(() => this._client.GetSortedSetCount(setId, fromScore, toScore), setId);
        }

        public long GetSortedSetCount(string setId, long fromScore, long toScore)
        {
            return this.TryCatch<long>(() => this._client.GetSortedSetCount(setId, fromScore, toScore), setId);
        }

        public long GetSortedSetCount(string setId, string fromStringScore, string toStringScore)
        {
            return this.TryCatch<long>(() => this._client.GetSortedSetCount(setId, fromStringScore, toStringScore), setId);
        }


        /// <summary>
        /// 从hash获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValueFromHash<T>(string hashId, string key)
        {
            return this.TryCatch<T>(() => JsonSerializer.DeserializeFromString<T>(this._client.GetValueFromHash(hashId, key)), hashId);
        }

        public string GetSubstring(string key, int fromIndex, int toIndex)
        {
            return this.TryCatch<string>(delegate
            {
                byte[] bytes = ((RedisClient)this._client).GetRange(key, fromIndex, toIndex);
                if (bytes != null)
                {
                    return StringExtensions.FromUtf8Bytes(bytes);
                }
                return null;
            }, key);
        }

        public TimeSpan GetTimeToLive(string key)
        {
            return this.TryCatch<TimeSpan>(delegate
            {
                TimeSpan? t = this._client.GetTimeToLive(key);
                if (!t.HasValue)
                {
                    return TimeSpan.Zero;
                }
                return t.Value;
            }, key);
        }

        public HashSet<string> GetUnionFromSets(params string[] setIds)
        {
            return this.TryCatch<HashSet<string>>(() => this._client.GetUnionFromSets(setIds), setIds[0]);
        }

        public string GetValue(string key)
        {
            return this.TryCatch<string>(() => this._client.GetValue(key), key);
        }

        public string GetValueFromHash(string hashId, string key)
        {
            return this.TryCatch<string>(() => this._client.GetValueFromHash(hashId, key), hashId);
        }

        public List<string> GetValues(List<string> keys)
        {
            return this.TryCatch<List<string>>(() => this._client.GetValues(keys), keys[0]);
        }

        public List<T> GetValues<T>(List<string> keys)
        {
            return this.TryCatch<List<T>>(() => this._client.GetValues<T>(keys), keys[0]);
        }

        public List<string> GetValuesFromHash(string hashId, params string[] keys)
        {
            return this.TryCatch<List<string>>(() => this._client.GetValuesFromHash(hashId, keys), hashId);
        }

        public Dictionary<string, T> GetValuesMap<T>(List<string> keys)
        {
            return this.TryCatch<Dictionary<string, T>>(() => this._client.GetValuesMap<T>(keys), keys[0]);
        }

        public Dictionary<string, string> GetValuesMap(List<string> keys)
        {
            return this.TryCatch<Dictionary<string, string>>(() => this._client.GetValuesMap(keys), keys[0]);
        }


        #endregion

        #region 各种移除


        public bool RemoveEntry(params string[] args)
        {
            return this.TryCatch<bool>(() => this._client.RemoveEntry(args), args[0]);
        }

        public bool RemoveEntryFromHash(string hashId, string key)
        {
            return this.TryCatch<bool>(() => this._client.RemoveEntryFromHash(hashId, key), hashId);
        }

        public long RemoveItemFromList(string listId, string value)
        {
            return this.TryCatch<long>(() => this._client.RemoveItemFromList(listId, value), listId);
        }

        public long RemoveItemFromList(string listId, string value, int noOfMatches)
        {
            return this.TryCatch<long>(() => this._client.RemoveItemFromList(listId, value, noOfMatches), listId);
        }

        public void RemoveItemFromSet(string setId, string item)
        {
            this.TryCatch(delegate
            {
                this._client.RemoveItemFromSet(setId, item);
            }, setId);
        }

        public bool RemoveItemFromSortedSet(string setId, string value)
        {
            return this.TryCatch<bool>(() => this._client.RemoveItemFromSortedSet(setId, value), setId);
        }

        public long RemoveRangeFromSortedSet(string setId, int minRank, int maxRank)
        {
            return this.TryCatch<long>(() => this._client.RemoveRangeFromSortedSet(setId, minRank, maxRank), setId);
        }

        public long RemoveRangeFromSortedSetByScore(string setId, double fromScore, double toScore)
        {
            return this.TryCatch<long>(() => this._client.RemoveRangeFromSortedSetByScore(setId, fromScore, toScore), setId);
        }

        public long RemoveRangeFromSortedSetByScore(string setId, long fromScore, long toScore)
        {
            return this.TryCatch<long>(() => this._client.RemoveRangeFromSortedSetByScore(setId, fromScore, toScore), setId);
        }

        public string RemoveStartFromList(string listId)
        {
            return this.TryCatch<string>(() => this._client.RemoveStartFromList(listId), listId);
        }

        public void RenameKey(string fromName, string toName)
        {
            this.TryCatch(delegate
            {
                this._client.RenameKey(fromName, toName);
            }, string.Empty);
        }


        #endregion

        #region 通过信道发布消息
        public long PublishMessage(string toChannel, string message)
        {
            return this.TryCatch<long>(() => this._client.PublishMessage(toChannel, message), string.Empty);
        }

        #endregion

        #region 搜索keys

        public List<string> SearchKeys(string pattern)
        {
            return this.TryCatch<List<string>>(() => this._client.SearchKeys(pattern), pattern);
        }
        #endregion

        #region 各种设置缓存
        
        public void SetAll(Dictionary<string, string> map)
        {
            this.TryCatch(delegate
            {
                this._client.SetAll(map);
            }, string.Empty);
        }

        public void SetAll(IEnumerable<string> keys, IEnumerable<string> values)
        {
            this.TryCatch(delegate
            {
                this._client.SetAll(keys, values);
            }, string.Empty);
        }

        public bool SetContainsItem(string setId, string item)
        {
            return this.TryCatch<bool>(() => this._client.SetContainsItem(setId, item), setId);
        }

        public void SetEntry(string key, string value)
        {
            this.TryCatch(delegate
            {
                this._client.SetValue(key, value);
            }, key);
        }

        public void SetEntry(string key, string value, TimeSpan expireIn)
        {
            this.TryCatch(delegate
            {
                this._client.SetValue(key, value, expireIn);
            }, key);
        }

        public bool SetEntryIfNotExists(string key, string value)
        {
            return this.TryCatch<bool>(() => this._client.SetValueIfNotExists(key, value), key);
        }

        public bool SetEntryInHash(string hashId, string key, string value)
        {
            return this.TryCatch<bool>(() => this._client.SetEntryInHash(hashId, key, value), hashId);
        }

        public bool SetEntryInHashIfNotExists(string hashId, string key, string value)
        {
            return this.TryCatch<bool>(() => this._client.SetEntryInHashIfNotExists(hashId, key, value), hashId);
        }

        public void SetItemInList(string listId, int listIndex, string value)
        {
            this.TryCatch(delegate
            {
                this._client.SetItemInList(listId, listIndex, value);
            }, listId);
        }

        public void SetRangeInHash(string hashId, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            this.TryCatch(delegate
            {
                this._client.SetRangeInHash(hashId, keyValuePairs);
            }, hashId);
        }

        public bool SortedSetContainsItem(string setId, string value)
        {
            return this.TryCatch<bool>(() => this._client.SortedSetContainsItem(setId, value), setId);
        }

        public void StoreAsHash<T>(T entity)
        {
            this.TryCatch(delegate
            {
                this._client.StoreAsHash<T>(entity);
            }, string.Empty);
        }


        public bool SetEntryInHash<T>(string hashId, string key, T value)
        {
            return this.TryCatch<bool>(() => this._client.SetEntryInHash(hashId, key, TextExtensions.SerializeToString<T>(value)), hashId);
        }
        public bool SetEntryInHashIfNotExists<T>(string hashId, string key, T value)
        {
            return this.TryCatch<bool>(() => this._client.SetEntryInHashIfNotExists(hashId, key, TextExtensions.SerializeToString<T>(value)), hashId);
        }


        #endregion

        #region Lock
        /// <summary>
        /// 获取锁？
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IDisposable AcquireLock(string key)
        {
            return this.TryCatch<IDisposable>(() => this._client.AcquireLock(key), key);
        }

        /// <summary>
        /// 获取锁？
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public IDisposable AcquireLock(string key, TimeSpan timeOut)
        {
            return this.TryCatch<IDisposable>(() => this._client.AcquireLock(key, timeOut), key);
        }


        #endregion

        #region 获取服务器时间

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetServerTime()
        {
            return this.TryCatch<DateTime>(() => this._client.GetServerTime(), string.Empty);
        }

        #endregion


        #region Microsoft.Extensions.Caching.Distributed.IDistributedCache

        /// <summary>
        /// 根据key获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte[] Get(string key)
        {
            return _client.Get<byte[]>(key);
        }

        /// <summary>
        /// GetAsync
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            return await Task.FromResult(_client.Get<byte[]>(key));
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var hasExpire = (options.AbsoluteExpirationRelativeToNow ?? options.SlidingExpiration).HasValue;
            if (hasExpire)
            {
                _client.Set(key, value, (options.AbsoluteExpirationRelativeToNow ?? options.SlidingExpiration).Value);
            }
            else
            {
                _client.Set(key, value);
            }
        }

        /// <summary>
        /// SetAsync
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            Set(key, value, options);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Refresh(string key)
        {
            //_client.Set(key);
            _client.GetTimeToLive(key);
            //_client.GetAndSetValue(key,value);
            //_client.Replace(key,value);
        }

        public async Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            Refresh(key);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 根据key删除
        /// </summary>
        /// <param name="key"></param>
        void IDistributedCache.Remove(string key)
        {
            _client.Remove(key);
        }

        /// <summary>
        /// 根据key异步删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            _client.Remove(key);
            await Task.CompletedTask;
        }


        #endregion


    }
}
