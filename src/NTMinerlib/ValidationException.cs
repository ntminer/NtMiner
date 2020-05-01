
namespace NTMiner {
    /// <summary>
    /// 在应用系统调用栈的顶层捕获异常，捕获异常时根据异常的类型如果是该类型则讲异常消息的描述字符串展示给用户。
    /// </summary>
    public class ValidationException : NTMinerException {
        public ValidationException(string message) : base(message) { }
    }
}
