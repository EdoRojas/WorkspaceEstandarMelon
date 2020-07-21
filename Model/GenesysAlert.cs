using Genesyslab.Desktop.Infrastructure.DependencyInjection;
using Genesyslab.Desktop.Infrastructure.Events;
using Microsoft.Practices.Composite.Events;

namespace Genesyslab.Desktop.Modules.InteractionExtensionCrossnet.Model
{
    public static class GenesysAlert
    {
        public static void SendMessage(string messageToSend, string sectionToSend, SeverityType severityTypeToSend)
        {
            var resultSentMessage = false;

            ContainerAccessPoint.Container.Resolve<IEventAggregator>().GetEvent<AlertEvent>().Publish(new Alert
            {
                Section = sectionToSend,
                Severity = severityTypeToSend,
                Id = messageToSend
            });

        }
    }
}