using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GirlsStandards
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [Obsolete]
    public class Server
    {
        public delegate string CommandMethod(string a);
        private readonly CommandMethod execution;
        internal static List<Server> servers = [];
        private Mutex mutex;
        public string ServerName { get; private set; }
        internal static string Global => (Initial.UseGlobals ? @"Global\" : "");

        private MemoryMappedFile mmf;
        private EventWaitHandle commandEvent;
        private EventWaitHandle responseEvent;
        private EventWaitHandle processingInProgress;

        public Server(string name, CommandMethod executingMethod)
        {
            execution = executingMethod;
            ServerName = Global + name.MakeAlphanumeral();
            SetupMMFF();
            servers.Add(this);
        }

        public static string MMFPath
            => OperatingSystem.IsLinux() ?
                "/dev/shm" :
                OperatingSystem.IsWindows() ?
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData),
                            "Celeste",
                            "MMF") :
                    throw new NotImplementedException(
                        "Unsupported Platform",
                        new PlatformNotSupportedException(
                            "Only supported platforms are linux and windows."
                        )
                    );

        [SupportedOSPlatform("windows")]
        [UnsupportedOSPlatform("linux", "Use sibling method for linux")]
        private void WinCreateFile()
        {
            new PlatformNotSupportedException("This method is for windows only, if using Linux see the sibling method: LinCreateFile.").ThrowIfNot(OperatingSystem.IsWindows());
            mmf = MemoryMappedFile.CreateNew(ServerName, 4096);
        }

        [SupportedOSPlatform("linux")]
        [UnsupportedOSPlatform("windows", "Use sibling method for windows.")]
        private void LinCreateFile()
        {
            new PlatformNotSupportedException("This method is for linux only. I suggest using WinCreateFile if on windows").ThrowIfNot(OperatingSystem.IsLinux());
            mmf = MemoryMappedFile.CreateFromFile($"/dev/shm/{ServerName}", FileMode.Create, ServerName, 4096);
        }

        private void CreateFile(string name)
            => ((Action)WinCreateFile).DoThisIfThatElseDoThis(OperatingSystem.IsWindows(), LinCreateFile);

        private static void SetupMMFF()
        {
            if (!Directory.Exists(MMFPath))
            {
                Logging.Log([$"MMF Path {MMFPath} not found, creating..."]);
                Directory.CreateDirectory(MMFPath);
            }
        }


        public bool OpenServer(string name)
        {
            string
                mutexName = Global + name + "_MMFmutex", 
                commandEventName = Global + name + "_MMFCommandEvent",
                responseEventName = Global + name + "_MMFResponseEvent",
                processingInProgressName = Global + name + "_MMFProcessingInProgress";

            try
            {
                var e = new InvalidOperationException("Handle already exists! Ensure this is closed before continuing.");
                mutex = new Mutex(false, mutexName, out bool good);
                e.ThrowIfNot(good);
                commandEvent = new EventWaitHandle(false, EventResetMode.AutoReset, commandEventName, out good);
                e.ThrowIfNot(good);
                responseEvent = new EventWaitHandle(false, EventResetMode.AutoReset, responseEventName, out good);
                e.ThrowIfNot(good);
                processingInProgress = new EventWaitHandle(false, EventResetMode.ManualReset, processingInProgressName, out good);
                e.ThrowIfNot(good);
                SetupMMFF();
                CreateFile(name);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to open server: " + ex.Message);
                return false;
            }
        }

        public void CloseServer()
        {
            commandEvent?.Dispose();
            responseEvent?.Dispose();
            processingInProgress?.Dispose();
            mutex?.Dispose();
            mmf?.Dispose();
            commandEvent = null;  
            responseEvent = null;  
            processingInProgress = null; 
            mutex = null; 
            mmf = null;
        }

        public string ProcessInput(string command)
            => execution(command) ?? "E4C4D4E.";

        public void RunServer()
        {
            //? Can't we just have a method called on an event invoked by a client that's signalling that theres a command to read?
            //todo: something to consider...
            while (true)
            {
                commandEvent.WaitOne();
                mutex.WaitOne();
                bool l0 = false;
                try
                {
                    processingInProgress.Set();
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        int length = accessor.ReadInt32(0);
                        if (length == 0)
                        {
                            l0 = true;
                            responseEvent.Set();
                            processingInProgress.Reset();
                            mutex.ReleaseMutex();
                            continue;
                        }

                        byte[] commandBytes = new byte[length];
                        accessor.ReadArray(4, commandBytes, 0, length);
                        string command = Encoding.UTF8.GetString(commandBytes);

                        string result = ProcessInput(command);
                        byte[] resultBytes = Encoding.UTF8.GetBytes(result);

                        accessor.Write(0, resultBytes.Length);
                        accessor.WriteArray(4, resultBytes, 0, resultBytes.Length);
                    }

                    if (!l0)
                        responseEvent.Set();
                }
                finally
                {
                    if (!l0)
                    {
                        processingInProgress.Reset();
                        mutex.ReleaseMutex();
                    }
                }
            }

        }
    }
}
