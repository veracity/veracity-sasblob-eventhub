# veracity-sasblob-eventhub
Code is describing how to transfer data from from csv file to Time Series Insight.

The scenario is as follows:

1. User has data in csv format stored in blob
2. User has access to that blob with SAS token
3. Application downloads csv blob, translates it to JSON format
4. Application sends JSON data to EventHub.
5. EventHub is connected to Time Series Insight as event source.
6. Data finaly lands in Time Series Insight.

Before executing this code user should have Time Series Insight environment set up together with EventHub connected to that TSI.

Useful links about creating and configuring Time Series Insight and EventHub

* https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-get-started
* https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-create
* https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-how-to-add-an-event-source-eventhub

This code is based on Microsoft tutorial:
* https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-send-events

NuGet packages used in solution:
* [ChoETL](https://github.com/Cinchoo/ChoETL)
* [ChoETL.JSON](https://github.com/Cinchoo/ChoETL)
* [WindowsAzure.ServiceBus](https://azure.microsoft.com/en-us/services/service-bus/)
* [WindowsAzure.Storage](https://docs.microsoft.com/en-us/azure/storage/)

# Usage
In Program.cs, Main method fill proper values in below code:
```csharp
var eventHubUrl = "< your endpoint url goes here >";
var sasUrl = "< sas url goes here >";
var blobName = "< csv blob name goes here >";
```
eventHubUrl is an Url taken from EventHub settings in Azure Portal

sasUrl is an Url with access token to container where CSV blob is stored

blobName is a name of CSV file together with extension

Downloading of content of CSV file is provided by below code
```csharp
var container = new CloudBlobContainer(new Uri(sasUrl));
var blockBlob = container.GetBlockBlobReference(blobName);
var csvString = blockBlob.DownloadText();
```
Translation from CSV to JSON is made with external [ChoETL](https://github.com/Cinchoo/ChoETL) library available as NuGet package.
```csharp
try
{
    string jsonResult;
    using (var p = new ChoCSVReader(new StringReader(csvContent)).WithFirstLineHeader().WithDelimiter(delimeter))
    {
        using (var stringWriter = new StringWriter())
        {
            using (var w = new ChoJSONWriter(stringWriter))
            {
                w.Write(p);
            }
            jsonResult = stringWriter.ToString();
        }
    }
    return jsonResult;
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
```

Sending to EventHub part is provided by external library [WindowsAzure.ServiceBus](https://azure.microsoft.com/en-us/services/service-bus/) wrapped here in separate class. 
```csharp
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
```