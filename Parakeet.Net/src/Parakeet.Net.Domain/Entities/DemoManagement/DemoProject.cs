using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 测试项目
    /// </summary>
    public class DemoProject : FullAuditedAggregateRoot<Guid>
    {
        //internal DemoProject(Guid id, string name) : base(id)
        //{
        //    Name = name;
        //}

        ////*
        // * 这里将Name的set 设置为private是为了演示领域根对象的操作是必须经过自身操作的
        // */
        //[MaxLength(255)] public string Name { get; private set; }

        //[MaxLength(255)] public string OldName { get; private set; }

        /////*
        //// * 使用IReadOnlyCollection确保此集合暴露给application层为只读，不允许修改或赋值
        //// */
        ////public IReadOnlyCollection<DemoUnitProject> DemoUnitProjects => _demoUnitProjects;

        /////*
        //// * 只允许领域根对象来操作其内部的领域实体
        //// */
        ////private HashSet<DemoUnitProject> _demoUnitProjects;

        ////*
        // * 此方法中不仅仅包含一个修改项目名称的操作，还需要更新OldName,限定了要修改项目名称就必须同步更改OldName
        // */
        ///// <summary>
        ///// 更新项目名称
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public DemoProject UpdateName(string name)
        //{
        //    //1.这里可以做一些业务逻辑上的判断
        //    //...
        //    //2.执行操作
        //    if (Name == name)
        //    {
        //        return this;
        //    }

        //    OldName = Name;
        //    Name = name;
        //    //3.下面可以发布一个领域事件，可以为本地事件或分布式事件
        //    //AddLocalEvent(new UpdateNameEto(OldName,Name));
        //    //or
        //    //AddDistributedEvent(new UpdateNameEto(OldName, Name));
        //    return this;
        //}

        /*
         * 演示如何操作领域对象中的成员实体对象
         */
        ///// <summary>
        ///// 同步项目与子项目的名称
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public DemoProject SynchronizationUnitProjectName(string name)
        //{
        //    UpdateName(name);
        //    if (_demoUnitProjects == null)
        //    {
        //        throw new AggregateException($"参数{nameof(_demoUnitProjects)}为调用included");
        //    }

        //    foreach (var demoUnitProject in _demoUnitProjects)
        //    {
        //        demoUnitProject.Name = name;
        //    }

        //    return this;
        //}
    }
}