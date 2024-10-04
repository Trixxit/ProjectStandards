using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace GirlsStandards
{
    public class Logging
    {
        public static Logging Instance { get; private set; }

        /// <summary>
        /// Sets the maximum queue size for <see cref="_logqueue"/>, which makes it autoflush when logs hit this number.
        /// </summary>
        private const int MaximumQueueSize = 50;

        /// <summary>
        /// How many minutes until <see cref="_logqueue"/> auto flushes (<see cref="FlushLogs"/>)
        /// </summary>
        private readonly TimeSpan FlushTime = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Thread-safe log queue.
        /// </summary>
        private ConcurrentQueue<string> _logqueue;

        /// <summary>
        /// SemaphoreSlim to allow only one thread to write to the file at a time.
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        /// Threading timer using <see cref="FlushTime"/> to auto call <see cref="FlushLogs"/>
        /// </summary>
        private Timer FlushTimer;

        public bool ExtensiveLogging { get; set; } = false;

        public bool CanLog { get; set; } = false;

        private string UID => Initial.instance.UID;

        /// <summary>
        /// The name of the file used for logging.
        /// </summary>
        public string LogFileName { get; private set; } = string.Empty;

        /// <summary>
        /// Sets up the logging functionality.
        /// </summary>
        /// <param name="path">The parent folder to log to</param>
        private void SetupLogs(string path)
        {
            string LogFolder = path;
            string name = string.Empty;
            if (path.Contains('.'))
            {
                name = Path.GetFileName(path);
                LogFolder = Path.GetDirectoryName(path);
            }

            if (Directory.Exists(LogFolder))
            {
                CanLog = true;
                if (name == string.Empty)
                    name = UID + ".log";
                LogFileName = Path.Combine(LogFolder, name);
                File.Create(LogFileName).Dispose();
            }
        }

        public Logging(string path, bool extensiveLogging)
        {
            if (Instance != null)
                return;
            LogFileName = path;
            ExtensiveLogging = extensiveLogging;
            Instance = this;
            SetupLogs(path);
        }

        /// <summary>
        /// Method to log a sequence of messages
        /// </summary>
        /// <param name="args">The messages to log</param>
        /// <returns></returns>
        [Obsolete]
        private async Task Log(params string[] args)
        {
            foreach (var arg in args)
            {
                Consowole.Instance.PrintLog(arg);
                await LogToFile(arg);
            }
        }

        /// <summary>
        /// Method that sends a string to the log queue for later file flushing.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns></returns>
        /// Auto flushes the log queue when at maximum queue limit
        private async Task LogToFile(string message)
        {
            if (!CanLog)
                return;
            _logqueue ??= new ConcurrentQueue<string>();
            _logqueue.Enqueue(message);
            if (_logqueue.Count >= MaximumQueueSize)
                await FlushLogs();
        }

        /// <summary>
        /// Method to flush all logs in the log queue to the log file.
        /// </summary>
        /// <returns></returns>
        public async Task FlushLogs()
        {
            if (_logqueue == null || _logqueue.IsEmpty)
                return;

            await _semaphore.WaitAsync();
            try
            {
                using var writer = new StreamWriter(LogFileName, append: true);
                while (_logqueue.TryDequeue(out var logMessage))
                    await writer.WriteLineAsync($"{DateTime.Now}: {logMessage}");

            }
            catch (Exception ex)
            {
                Consowole.Instance.PrintLog($"[bold red]Failed to flush logs: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Method to process a sequence of messages, and then log them.
        /// </summary>
        /// <param name="messages">The sequence of objects to process (turn into messages through <see cref="ProcessMessage(object)"/>) and then log.</param>
        /// <param name="memberName">The method name that called this log. [Auto-filled]</param>
        /// <param name="filePath">The file path to the file of the method that called this log. [Auto-filled]</param>
        /// <param name="lineNumber">The line number in the file of the method that called this log. [Auto-filled]</param>
        /// <returns></returns>
        /// <remarks>Has auto processing of complex objects. Also, don't put anything for the latter 3 parameters, these are auto-filled.</remarks>
        /// <example>Log(["womp womp", new Dictionary(string, string) { { "hi", "woah" }}])</example>
        [SuppressMessage("WarningCategory", "CS4014", Justification = "The call is fire-and-forget intentionally.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task Log(
            object[] messages,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0
        )
        {
            var tmstmp = $"{DateTime.Now:HH: mm: ss: fff}";
            foreach (var message in messages)
            {
                string[] str = ProcessMessage(message);
                foreach (string str2 in str)
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        Consowole.Instance?.PrintLog(((Instance?.ExtensiveLogging ?? false)? $"Called from {filePath} by {memberName} at line #{lineNumber}: " : "") + str2);
                        if (Instance is not null)
                            await Instance.LogToFile((Instance.ExtensiveLogging ? $"Called from {filePath} by {memberName} at line #{lineNumber}: " : "") + $"[{tmstmp}] {str2}");
                    }
            }
        }

        /// <summary>
        /// Similar to <see cref="Log(object[], string, string, int)"/> but specifically for exceptions. DO NOT USE THE OTHER FOR EXCEPTIONS IF YOU WANT ZE NICE FORMATTING.
        /// </summary>
        /// <param name="exc">The exception to process.</param>
        /// <param name="memberName">The method name that called this log. [Auto-filled]</param>
        /// <param name="filePath">The file path to the file of the method that called this log. [Auto-filled]</param>
        /// <param name="lineNumber">The line number in the file of the method that called this log. [Auto-filled]</param>
        public static async void LogError(
                Exception exc,
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string filePath = "",
                [CallerLineNumber] int lineNumber = 0
            )
        {
            var logEntry = new StringBuilder();
            var guid = Guid.NewGuid().ToString("N");
            List<string> sequence =
            [
                $">>>ERROR {guid} START<<<",
                $"Message: {exc.Message}",
                $"Source: {exc.Source}",
                $"Method Base: {exc.TargetSite?.Module}.{exc.TargetSite?.DeclaringType}.{exc.TargetSite?.Name} ({exc.TargetSite})",
                $"StackTrace: {exc.StackTrace}",
                $"Data: {string.Join(", ", exc.Data.Keys.Cast<object>().Zip(exc.Data.Values.Cast<object>(), (k, v) => $"  -  {k}: {v}")).Replace("\n", "   -   ")}",
                $"HLink: {exc.HelpLink}",
                $"HResult: {exc.HResult}",
                $">>>END OF ERROR {guid}<<<",
            ];
            string dt = $"[{DateTime.Now:HH:mm:ss:fff}]";
            if (Instance?.ExtensiveLogging ?? false)
            {
                sequence.Insert(0, $"At line: {lineNumber}");
                sequence.Insert(0, $"By member: {memberName}");
                sequence.Insert(0, $"Called from file {filePath}");
            }
            foreach (var item in sequence)
            {
                Consowole.Instance.PrintLog(item);
                logEntry.AppendLine($"[{dt}] {item}");
            }
            Consowole.Instance?.PrintLog(logEntry.ToString());
            if (Instance is not null)
                await Instance.LogToFile(logEntry.ToString());
            if (exc.InnerException != null)
                LogError(exc.InnerException);
        }

        /// <summary>
        /// Method that deconstructs a complex object into a human-friendly string representation.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Human readable string representation.</returns>
        /// <remarks>Only works on enumerables.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string[] ProcessMessage(object message)
        {
            if (message is System.Collections.IEnumerable enumerable && message is not string)
            {
                var sb = new StringBuilder();
                foreach (var item in enumerable)
                {
                    string[] pitem = ProcessMessage(item);
                    foreach (var item2 in pitem)
                        sb.AppendLine(item2);
                }
                return sb.ToString().Split("\n");
            }
            return [message?.ToString() ?? string.Empty];
        }

        internal async Task CloseLogs()
        {
            _semaphore.Dispose();
            await LogToFile("Closing program.");
            await FlushLogs();
            await FlushTimer.DisposeAsync();
        }
    }
}
