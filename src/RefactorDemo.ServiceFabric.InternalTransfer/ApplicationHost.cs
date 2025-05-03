using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace RefactorDemo.ServiceFabric.InternalTransfer
{
    [EventSource(Name = "MyCompany-RefactorDemo-InternalTransfer")]
    internal sealed class ServiceEventSource : EventSource
    {
        public static readonly ServiceEventSource Current = new ServiceEventSource();

        [Event(1, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}")]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(1, hostProcessId, serviceType);
        }

        [Event(2, Level = EventLevel.Error, Message = "Service host initialization failed {0}")]
        public void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent(2, exception);
        }
    }

    internal static class ApplicationHost
    {
        private static void Main()
        {
            try
            {
                ServiceRuntime.RegisterServiceAsync("InternalTransferServiceType",
                    context => new InternalTransferServiceHost(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(InternalTransferServiceHost).Name);

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }

    internal sealed class InternalTransferServiceHost : Microsoft.ServiceFabric.Services.Runtime.StatelessService
    {
        public InternalTransferServiceHost(StatelessServiceContext context)
            : base(context)
        {
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var accountRepository = new RefactorDemo.Adapters.AccountRepository();
            var transferService = new RefactorDemo.Domain.InternalTransferService(accountRepository, Context);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }
    }
}
