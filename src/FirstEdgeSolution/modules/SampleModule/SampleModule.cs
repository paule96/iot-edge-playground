using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EdgeHub;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.Samples
{
    public class SampleModule
    {
        private const string sqlTimeFormate = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
        private readonly SqlConnection connection;

        public SampleModule(SqlConnection connection)
        {
            this.connection = connection;
        }
        [FunctionName("SampleModule")]
        public async Task FilterMessageAndSendMessage(
                    [EdgeHubTrigger("input1")] Message messageReceived,
                    [EdgeHub(OutputName = "output1")] IAsyncCollector<Message> output,
                    ILogger logger)
        {
            byte[] messageBytes = messageReceived.GetBytes();
            var messageString = System.Text.Encoding.UTF8.GetString(messageBytes);

            if (!string.IsNullOrEmpty(messageString))
            {
                // Get the body of the message and deserialize it.
                var messageBody = JsonConvert.DeserializeObject<MachineEvent>(messageString);
                var insertTemperature = @"INSERT INTO dbo.Temperature VALUES (@Timestamp, @Temperature, @Type);";
                
                var parsedDate = DateTime.Parse(messageBody.TimeCreated.ToString(sqlTimeFormate, CultureInfo.InvariantCulture));
                using (SqlCommand cmd = new SqlCommand(insertTemperature, connection))
                {
                    var timeParam = new SqlParameter("@Timestamp", SqlDbType.DateTime2);
                    timeParam.Value = parsedDate;
                    cmd.Parameters.Add(timeParam);
                    cmd.Parameters.Add(new SqlParameter("@Temperature", messageBody.Ambient.Temperature));
                    cmd.Parameters.Add(new SqlParameter("@Type", "Abient"));
                    //Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    logger.LogInformation($"{rows} rows were updated");
                }
                using (SqlCommand cmd = new SqlCommand(insertTemperature, connection))
                {
                    var timeParam = new SqlParameter("@Timestamp", SqlDbType.DateTime2);
                    timeParam.Value = parsedDate;
                    cmd.Parameters.Add(timeParam);
                    cmd.Parameters.Add(new SqlParameter("@Temperature", messageBody.Machine.Temperature));
                    cmd.Parameters.Add(new SqlParameter("@Type", "Machine"));
                    //Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    logger.LogInformation($"{rows} rows were updated");
                }
                logger.LogInformation("Info: Received one non-empty message");
                using (var pipeMessage = new Message(messageBytes))
                {
                    foreach (KeyValuePair<string, string> prop in messageReceived.Properties)
                    {
                        pipeMessage.Properties.Add(prop.Key, prop.Value);
                    }
                    await output.AddAsync(pipeMessage);
                    logger.LogInformation("Info: Piped out the message");
                }
            }
        }
    }

    class MachineEvent
    {
        public Machine Machine;
        public Ambient Ambient;
        public DateTimeOffset TimeCreated;
    }

    class Machine
    {
        public float Temperature;
        public float pressure;
    }

    class Ambient
    {
        public float Temperature;
        public uint Humidity;
    }
}
