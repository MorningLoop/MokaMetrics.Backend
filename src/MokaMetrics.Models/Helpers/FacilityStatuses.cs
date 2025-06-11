namespace MokaMetrics.Models.Helpers;

public enum FacilityStatuses
{
    Operational = 1, // Facility is operational and running normally
    Maintenance = 2, // Facility is under maintenance
    Alarm = 3, // Facility is under alarm or has a critical issue
    Shutdown = 4, // Facility is shut down
    Idle = 5, // Facility is idle and not in production
}
