using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.SequenceGate;
using NServiceBus.SequenceGate.Tests.Acceptance.Messages;

class ProgramService : ServiceBase
{
    IBus bus;

    static ILog logger = LogManager.GetLogger<ProgramService>();

    static void Main()
    {
        using (var service = new ProgramService())
        {
            // so we can run interactive from Visual Studio or as a windows service
            if (Environment.UserInteractive)
            {
                Console.CancelKeyPress += (sender, e) =>
                {
                    service.OnStop();
                };
                service.OnStart(null);
                Console.WriteLine("\r\nPress enter key to stop program\r\n");
                Console.Read();
                service.OnStop();
                return;
            }
            Run(service);
        }
    }

    public void Customize(BusConfiguration configuration)
    {
        var sequenceGateConfiguration = new SequenceGateConfiguration
        {
            new SequenceGateMember
            {
                Id = "UserEmailUpdated",
                Messages = new List<MessageMetadata>
                {
                    new MessageMetadata
                    {
                        Type = typeof(UserEmailUpdated),
                        ObjectIdPropertyName = nameof(UserEmailUpdated.UserId),
                        TimeStampPropertyName = nameof(UserEmailUpdated.TimeStampUtc)
                    }
                }
            },
            new SequenceGateMember
            {
                Id = "VIPStatusActions",
                Messages = new List<MessageMetadata>
                {
                    new MessageMetadata
                    {
                        Type = typeof(VIPStatusGranted),
                        ObjectIdPropertyName = nameof(User.Id),
                        CollectionPropertyName = nameof(VIPStatusGranted.Users),
                        TimeStampPropertyName = nameof(VIPStatusGranted.TimeStamp)
                    },
                    new MessageMetadata
                    {
                        Type = typeof(VIPStatusRevoked),
                        CollectionPropertyName = nameof(VIPStatusRevoked.UserIds),
                        TimeStampPropertyName = nameof(VIPStatusRevoked.TimeStamp)
                    }
                }
            }
        };

        configuration.SequenceGate(sequenceGateConfiguration);
    }
    protected override void OnStart(string[] args)
    {
        try
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("NServiceBus.SequenceGate.Tests.Acceptance.Endpoint");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.DefineCriticalErrorAction(OnCriticalError);
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();
            Customize(busConfiguration);
            var startableBus = Bus.Create(busConfiguration);
            bus = startableBus.Start();
        }
        catch (Exception exception)
        {
            OnCriticalError("Failed to start the bus.", exception);
        }
    }

    void OnCriticalError(string errorMessage, Exception exception)
    {
        //TODO: Decide if shutting down the process is the best response to a critical error
        //http://docs.particular.net/nservicebus/hosting/critical-errors
        var fatalMessage = string.Format("The following critical error was encountered:\n{0}\nProcess is shutting down.", errorMessage);
        logger.Fatal(fatalMessage, exception);
        Environment.FailFast(fatalMessage, exception);
    }

    protected override void OnStop()
    {
        if (bus != null)
        {
            bus.Dispose();
        }
    }

}