using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using System.Security;
using System.Text;

namespace GirlsStandards
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [Obsolete]
    public class Client
    {
        public string ServerName { get; private set; }
        public bool Ready { get; private set; } = false;
        internal static string Global => Server.Global;

        private readonly Mutex mutex;
        private readonly EventWaitHandle commandEvent, responseEvent, processingInProgress;

        private static bool VerifyServer(string name)
        {
            if (OperatingSystem.IsWindows())
                try
                {
                    _ = MemoryMappedFile.OpenExisting(name);
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch
                {
                    return false;
                }
            else if (OperatingSystem.IsLinux())
                try
                {
                    _ = MemoryMappedFile.CreateFromFile($"/dev/shm/{name}", FileMode.Open, name);
                }
                catch (SecurityException)
                {
                    return false;
                }
                catch
                {
                    return false;
                }
            return true;
        }

        public Client(string serverName)
        {
            ServerName = Global + serverName.MakeAlphanumeral();
            Ready = VerifyServer(serverName);
            if (!Ready)
            {
                Console.WriteLine($"There is no open server with name {serverName}, this client is now obsolete.");
                return;
            }
            string mutexName = Global + serverName + "_MMFmutex";
            string commandEventName = Global + serverName + "_CommandEvent";
            string responseEventName = Global + serverName + "_ResponseEvent";
            string processingInProgressName = Global + serverName + "_ProcessingInProgress";
            bool s = Mutex.TryOpenExisting(mutexName, out mutex);
            new InvalidOperationException($"Failed to obtain server mutex.").ThrowIfNot(s);
            s = EventWaitHandle.TryOpenExisting(commandEventName, out commandEvent);
            new InvalidOperationException($"Failed to obtain server command event.").ThrowIfNot(s);
            s = EventWaitHandle.TryOpenExisting(responseEventName, out responseEvent);
            new InvalidOperationException($"Failed to obtain server response event.").ThrowIfNot(s);
            s = EventWaitHandle.TryOpenExisting(processingInProgressName, out processingInProgress);
            new InvalidOperationException($"Failed to obtain server progress event.").ThrowIfNot(s);
        }

        private static MemoryMappedFile FindMMF(string mapName)
        {
            if (OperatingSystem.IsWindows())
                return MemoryMappedFile.OpenExisting(mapName);
            else if (OperatingSystem.IsLinux())
                return MemoryMappedFile.CreateFromFile($"/dev/shm/{mapName}", FileMode.Open, mapName);
            return null;
        }

        public Task<string> QueueCommandReceiveOutput(string command)
        {
            TaskCompletionSource<string> tcs = new();
            if (!Ready)
            {
                tcs.SetResult("Client is not connected.");
                return tcs.Task;
            } 
            processingInProgress.WaitOne();
            mutex.WaitOne();
            try
            {
                using (var mmf = FindMMF(ServerName))
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    byte[] commandBytes = Encoding.UTF8.GetBytes(command);
                    accessor.Write(0, commandBytes.Length);
                    accessor.WriteArray(4, commandBytes, 0, commandBytes.Length);
                    commandEvent.Set();
                }
                responseEvent.WaitOne();
                using (var mmf = FindMMF(ServerName))
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    int length = accessor.ReadInt32(0);
                    byte[] outputBytes = new byte[length];
                    accessor.ReadArray(4, outputBytes, 0, length);
                    accessor.Write(0, 0);
                    tcs.SetResult(Encoding.UTF8.GetString(outputBytes));
                    return tcs.Task;
                }
            }
            finally
            {
                processingInProgress.Set();
                mutex.ReleaseMutex();
            }
        }
    }
}
