using Microsoft.Extensions.Logging;

namespace Migration.Contracts
{
    /// <summary>
    /// Extensions for logger:)
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log message for error with adding employee
        /// </summary>
        public static void LogAddEmployeeError(this ILogger logger, string serviceName, Exception ex)
        {
            logger.LogError(ex, "{Service} Failed to add employee: {ErrorMessage}", serviceName, ex.Message);
        }

        /// <summary>
        /// Log message for error with removing employee
        /// </summary>
        public static void LogRemoveEmployeeError(this ILogger logger, string serviceName, Guid employeeId, Exception ex)
        {
            logger.LogError(ex, "{Service} Failed to remove employee {EmployeeId}: {ErrorMessage}", serviceName, employeeId, ex.Message);
        }

        /// <summary>
        /// Log message for error with updating employee
        /// </summary>
        public static void LogUpdateEmployeeError(this ILogger logger, string serviceName, Guid employeeId, Exception ex)
        {
            logger.LogError(ex, "{Service} Failed to update employee {EmployeeId}: {ErrorMessage}", serviceName, employeeId, ex.Message);
        }
    }
}
