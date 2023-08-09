using System.Diagnostics;

namespace shutdownApi.Services
{
    public class LinuxShutdownService : IShutdownService
    {
        public void Halt()
        {
            using var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "/bin/sudo",
                    ArgumentList = { "-n", "halt" }
                });
        }

        public void PowerOff()
        {
            using var process = Process.Start(
            new ProcessStartInfo
            {
                FileName = "/bin/sudo",
                ArgumentList = { "-n", "poweroff" }
            });
        }
    }
}
