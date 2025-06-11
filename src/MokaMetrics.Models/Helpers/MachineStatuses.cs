namespace MokaMetrics.Models.Helpers;

public enum MachineStatuses
{
    Operational = 1, // Machine is operational and running normally
    Maintenance = 2, // Machine is under maintenance
    Alarm = 3, // Machine is under alarm or has a critical issue
    Offline = 4, // Machine is shut down
    Idle = 5 // Machine is idle and not in production
}
