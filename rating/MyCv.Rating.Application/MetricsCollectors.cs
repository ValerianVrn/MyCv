using Prometheus;

namespace MyCv.Rating.Application
{
    internal static class MetricsCollectors
    {
        public const string MessageHandlingReceivedState = "Received";
        public const string MessageHandlingHandledState = "Handled";

        /// <summary>
        /// Histogram for the duration of the execution of a command.
        /// </summary>
        internal static readonly Histogram CommandExecutionDurationHistogram = Metrics.CreateHistogram("mycv_command_duration_seconds", "Histogram of command execution duration (in s).",
                                                                                                      new HistogramConfiguration
                                                                                                      {
                                                                                                          Buckets = Histogram.PowersOfTenDividedBuckets(startPower: -3, endPower: 0, divisions: 10),
                                                                                                          LabelNames = ["CommandType"]
                                                                                                      });
        /// <summary>
        /// Counter of the executed commands.
        /// </summary>
        internal static readonly Counter CommandExecutionCounter = Metrics.CreateCounter("mycv_command_total", "Total number of commands executed.",
                                                                                                      new CounterConfiguration
                                                                                                      {
                                                                                                          LabelNames = ["CommandType"]
                                                                                                      });
    }
}
