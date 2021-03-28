using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Linq;
using MS.model;

namespace MS.Function
{
    public static class receivemail
    {
        [FunctionName("receivemail")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [Queue("outqueue"),StorageAccount("AzureWebJobsStorage")] ICollector<Message> msg, 
              ExecutionContext executionContext)
        {
            //receive post message
            string inputData = await req.Content.ReadAsStringAsync();
            //convert message to object
            Message message = JsonConvert.DeserializeObject<Message>(inputData);
            //remove duplicates from atributes
            message.Attributes = message.Attributes.Distinct().ToList();
            //add message to queue
            msg.Add(message);
            //send ok status code
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}