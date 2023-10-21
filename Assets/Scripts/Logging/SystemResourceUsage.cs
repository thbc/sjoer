#if ENABLE_WINMD_SUPPORT
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.System.Diagnostics;

public class SystemResourceUsage
{
    public static async Task<float> GetCpuUsage()
    {
        var cpuReport = SystemDiagnosticInfo.GetForCurrentSystem().CpuUsage.GetReport();
        var userUsage = cpuReport.UserTime.Duration();
        var idleUsage = cpuReport.IdleTime.Duration();
        var kernelUsage = cpuReport.KernelTime.Duration();  // Includes UserTime

        var total = userUsage + idleUsage + kernelUsage;

        if (total.Ticks == 0) // to avoid division by zero
            return 0;

        // Calculate the CPU usage percentage
        var usagePercentage = (float)((userUsage + kernelUsage - idleUsage).Ticks * 100) / total.Ticks;

        return usagePercentage;
    }

    public static ulong GetMemoryUsage()
    {
        // This method gets the app's current memory usage.
        var memoryReport = MemoryManager.GetAppMemoryReport();

        // You can access various properties of the memory report here.
        // For simplicity, we're returning the Total Commit Usage.
        return memoryReport.TotalCommitUsage;
    }
}
#endif
