using System.Runtime.Serialization;

namespace NilArea.Contracts.Exceptions;

/// <summary>
/// 权限相关异常
/// </summary>
[Serializable]
[GenerateSerializer]
[Alias("NilArea.Contracts.Exceptions.PermissionException")]
public class PermissionException : OrleansException
{
    /// <summary>
    /// 初始化 <see cref="PermissionException"/> 类的新实例
    /// </summary>
    public PermissionException()
    {
    }

    /// <summary>
    /// 使用指定的错误消息初始化 <see cref="PermissionException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    public PermissionException(string message) : base(message)
    {
    }

    /// <summary>
    /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="PermissionException"/> 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="inner">导致当前异常的异常</param>
    public PermissionException(string message, Exception inner) : base(
        message, inner)
    {
    }

    /// <summary>
    /// 用序列化数据初始化 <see cref="PermissionException"/> 类的新实例
    /// </summary>
    /// <param name="info">保存序列化对象数据的对象</param>
    /// <param name="context">有关源或目标的上下文信息</param>
    [Obsolete]
    protected PermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}