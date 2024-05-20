using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Parakeet.Net.Repositories
{
    ///// <summary>
    ///// 自定义操作单元--TransactionScope--单数据库多连接对象的事务
    ///// </summary>
    //public class CustomUnitOfWork : ICustomUnitOfWork
    //{
    //    public IServiceProvider ServiceProvider { get; }
    //    public IDatabaseApi FindDatabaseApi(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void AddDatabaseApi(string key, IDatabaseApi api)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITransactionApi FindTransactionApi(string key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void AddTransactionApi(string key, ITransactionApi api)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetOuter(IUnitOfWork outer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Initialize(AbpUnitOfWorkOptions options)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Reserve(string reservationName)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task CompleteAsync(CancellationToken cancellationToken = new CancellationToken())
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnCompleted(Func<Task> handler)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Guid Id { get; }
    //    public IAbpUnitOfWorkOptions Options { get; }
    //    public IUnitOfWork Outer { get; }
    //    public bool IsReserved { get; }
    //    public bool IsDisposed { get; }
    //    public bool IsCompleted { get; }
    //    public string ReservationName { get; }
    //    public event EventHandler<UnitOfWorkFailedEventArgs> Failed;
    //    public event EventHandler<UnitOfWorkEventArgs> Disposed;
    //}
}
