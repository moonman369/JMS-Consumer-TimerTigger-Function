using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using JMS_Consumer_TimerTigger_Function.Utils.MQFunctions;
using System.Threading.Tasks;


namespace JMS_Consumer_TimerTigger_Function
{
    public class JMSConsumerTimerTiggerFunction
    {
        [FunctionName("JMSConsumerTimerTiggerFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var fetchedMessages = await MQUtils.ReceiveMessages();

            foreach (var fetchedMessage in fetchedMessages)
            {
                log.LogInformation(fetchedMessage.Text);
            }

            log.LogInformation($"\n===================\n|    Count : {fetchedMessages.Count}    |\n===================");
        }
    }
}
