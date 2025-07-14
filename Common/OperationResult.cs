using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum Result
    {
        Success,
        NotFound,
        Forbidden
    }
    public class OperationResult<T>
    {
        public T? Value { get; private set; }
        public Result Status { get; private set; }
        public bool IsSuccess => Status == Result.Success;

        public static OperationResult<T> Success(T value)
        {
            return new OperationResult<T> { Value = value, Status = Result.Success };
        }

        public static OperationResult<T> Fail(Result errorType)
        {
            if (errorType == Result.Success)
                throw new ArgumentException("Error type must not be Success.", nameof(errorType));

            return new OperationResult<T> { Value = default, Status = errorType };
        }
    }
}
