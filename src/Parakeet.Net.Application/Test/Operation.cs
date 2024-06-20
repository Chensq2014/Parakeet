using System;
using Common.Test;

namespace Parakeet.Net.Test
{
    public class Operation : IOperationTransient, IOperationScoped, IOperationSingleton
    {
        public Operation()
        {
            OperationId = Guid.NewGuid().ToString();
        }
        public string OperationId { get; }
    }
}
