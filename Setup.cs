using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Avalonia.Threading;
using System.Runtime.CompilerServices;
using Avalonia;

namespace GirlsStandards
{
    public sealed partial class Initial
    {
        public static bool UseGlobals { get; set; } = false;

        /// <summary>
        /// Singluar instance of the console for use. Ensures setup is complete without having a ton of static methods with flags and checks.
        /// </summary>
        internal static Initial? instance = null;

        /// <summary>
        /// Internal uid field, given a halved GUID as a hash.
        /// </summary>
        private string _uid = BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()))).Replace("-", "")[..16];

        /// <summary>
        /// The UID of this given common console instance, may be used for logging and unique server names.
        /// </summary>
        public string UID 
            => _uid;

        /// <summary>
        /// Factory setup method to initialize and... setup a common console instance given a preset <see cref="SetupInfo"/>.
        /// </summary>
        /// <param name="setup"></param>
        /// <returns>The instance</returns>
        /// <remarks>If called more than once, will only return the already open instance.</remarks>
        public static Initial Setup(bool useGlobals = false, string customUID = "")
        {
            if (instance != null)
                return instance;
            instance = new Initial(customUID);
            UseGlobals = useGlobals;
            BuildAvaloniaApp();
            return instance;
        }

        internal static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                 .UsePlatformDetect()
                 .LogToTrace();

        /// <summary>
        /// Internal constructor for actually creating the console instance.
        /// </summary>
        private Initial(string customUID)
            => _uid = !string.IsNullOrWhiteSpace(customUID) ? customUID : _uid;

        /// <summary>
        /// PLEASE call this method whenever your app closes so that it flushes logs and disposes of any memory consuming annoyances :3
        /// </summary>
        /// <remarks>Await this method to ensure that everything disposes correctly</remarks>
        public async Task Close()
        {
            foreach (Server server in Server.servers)
                server.CloseServer();
            await Consowole.Instance.CloseConsole();
            await Logging.Instance.CloseLogs();
        }
    }
}
