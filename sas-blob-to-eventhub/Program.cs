using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace sas_blob_to_eventhub
{
    /// <summary>
    /// Main class showing the simple workflow:
    /// 1. Download csv data from blob with SAS token
    /// 2. Convert data to json
    /// 3. Send to event hub
    /// Before executing this code you should have Time Series Insight environment set up together
    /// with EventHub connected to that TSI.
    /// https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-get-started
    /// https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-create
    /// https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-how-to-add-an-event-source-eventhub
    /// This code is based on Microsoft tutorial:
    /// https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-send-events
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            var eventHubUrl = "< your endpoint url goes here >";
            var sasUrl = "< sas url goes here >";
            var blobName = "< csv blob name goes here >";
            try
            {
                var container = new CloudBlobContainer(new Uri(sasUrl));
                var blockBlob = container.GetBlockBlobReference(blobName);
                var csvString = blockBlob.DownloadText();
                Console.WriteLine("Csv content downloaded. Converting to json.");
                var jsonString = CsvToJsonConverter.Convert(csvString, ",");
                Console.WriteLine("Json created. Uploading to event hub.");
                var eManager = new EventHubManager(eventHubUrl);
                eManager.SendJson(jsonString);
                Console.WriteLine("Json sent to EventHub.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured: "+ e);
            }
        }
    }
}