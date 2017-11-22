using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace sas_blob_to_eventhub
{
    /// <summary>
    /// Class responsible for contact with event hub.
    /// EventHub needs to be defined and url to eventHub need to be passed via constructor.
    /// This code is based on Microsoft tutorial:
    /// https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-send-events
    /// </summary>
    public class EventHubManager
    {
        private readonly EventHubClient _eventHubClient;
        public EventHubManager(string eventHubConnectionString)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString);
        }
        /// <summary>
        /// Sends json string to event hub.
        /// </summary>
        /// <param name="json">string json</param>
        public void SendJson(string json)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            {
                sw.Write(json);
                sw.Flush();
                ms.Position = 0;
                var eventData = new EventData(ms);
                _eventHubClient.Send(eventData);
            }
        }
    }
}