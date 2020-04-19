using System.Threading.Tasks;

namespace NTMiner {
    public static class TaskEx {
        private static readonly Task _completedTask = TaskEx.FromResult(false);
        public static Task CompletedTask {
            get {
                return _completedTask;
            }
        }

        public static Task<T> FromResult<T>(T result) {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            Task<T> task = tcs.Task;
            tcs.SetResult(result);
            return task;
        }
    }
}
