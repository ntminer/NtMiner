using System.Threading.Tasks;

namespace NTMiner {
    public static class TaskEx {
        public static Task CompletedTask { get; private set; } = FromResult(false);

        public static Task<T> FromResult<T>(T result) {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            Task<T> task = tcs.Task;
            tcs.SetResult(result);
            return task;
        }
    }
}
