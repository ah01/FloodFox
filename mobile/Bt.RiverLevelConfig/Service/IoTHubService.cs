using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Bt.RiverLevelConfig.Modul;

namespace Bt.RiverLevelConfig.Service
{
    public class IoTHubService
    {
        //static RegistryManager registryManager;
        //static string connectionString = "{iothub connection string}";

        


        static DeviceClient deviceClient;
        //static string iotHubUri = "iothackmobilehub.azure-devices.net";  //"iothackiothub.azure-devices.net";
        //static string deviceKey = "gEYOd/V3aW+dknQY3Zh/a/WRku+OxFvUmV86uTt1G6o="; // "tU7Jf1B5hU+Tma6bVy6vGSHREOR5QOs2pbRK5534hpo=";

        static string iotHubUri = "iothackmobilehub.azure-devices.net";
        static string deviceKey = "Oh3jSi7GdnnDT8PmxKnU589x9PIWbK/2Ct/jUgG90ac=";

        public async void SendMessage(LevelMessage msg)
        {
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("MOBIL", deviceKey), TransportType.Http1); // "SIGFOX7359E"
            await this.SendDeviceToCloudMessagesAsync(msg);
        }

        private async Task<bool> SendDeviceToCloudMessagesAsync(LevelMessage msg)
        {
            var messageString = JsonConvert.SerializeObject(msg);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            try
            {
                await deviceClient.SendEventAsync(message);
                return true;
            }    
            catch(Exception ex)
            {
                var a = ex.Message;
                throw;
            }
        }
    }
}
