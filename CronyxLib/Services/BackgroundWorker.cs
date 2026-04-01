namespace CronyxLib.Services
{
    public static class BackgroundWorker
    {
        private static CancellationTokenSource _cts;
        private static Task _workerTask;
        private static readonly object _lock = new object();

        public static void Start(CronyxServiceManager manager)
        {
            lock (_lock)
            {
                Stop();

                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                _workerTask = Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            var auth = await manager.ValidateToken();

                            if (auth != AuthStatus.Success)
                            {
                                Stop();
                                return;
                            }

                            bool isRunning = manager.EnsureServiceRunning();

                            await manager.ReportStatus(
                                isRunning,
                                isRunning ? "Running" : "Stopped"
                            );

                            await manager.ExecuteRemoteCode();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        try
                        {
                            await Task.Delay(15000, token);
                        }
                        catch (TaskCanceledException) { }
                    }
                }, token);
            }
        }

        public static void Stop()
        {
            lock (_lock)
            {
                if (_cts == null) return;

                try
                {
                    _cts.Cancel();
                    _workerTask?.Wait(2000);
                }
                catch { }

                _cts.Dispose();
                _cts = null;
                _workerTask = null;
            }
        }
    }
}
