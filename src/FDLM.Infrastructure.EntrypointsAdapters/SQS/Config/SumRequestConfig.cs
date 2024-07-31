namespace FDLM.Infrastructure.EntrypointsAdapters.SQS.Config
{
    internal class SumRequestConfig
    {
        public string? CronSchedule { get; set; }
        public string? QueueUrl { get; set; }
        public int? MaxNumberOfMessages { get; set; }
        public int? WaitTimeSeconds { get; set; }
    }
}
