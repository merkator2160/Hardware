using IotHub.Api.Services.Models.Messages;
using IotHub.Common.Const;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotHub.Api.Services.Mqtt
{
    internal partial class MosquittoClient
    {
        // TOPIC REGISTRATION /////////////////////////////////////////////////////////////////////
        public void AddButtonHandlers(Dictionary<String, MqttClient.MqttMsgPublishEventHandler> handlerDictionary)
        {
            handlerDictionary.Add($"zigbee/{ZigbeeDevice.TuyaButtonPad4}", OnTuyaButtonPad4MessageReceived);
            handlerDictionary.Add($"zigbee/{ZigbeeDevice.ModkamButtonPad12}", OnButtonPad12MessageReceived);
            handlerDictionary.Add($"zigbee/{ZigbeeDevice.SideRoomGreenhouseLightButton}", OnSideRoomKaktusLightButtonMessageReceived);
            handlerDictionary.Add($"zigbee/{ZigbeeDevice.Button1}", OnButton1MessageReceived);
        }


        // HANDLERS ///////////////////////////////////////////////////////////////////////////////
        private void OnTuyaButtonPad4MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
            var message = JsonConvert.DeserializeObject<TuyaButtonPadMsg>(jsonStr);

            if (message.Action.Equals(TuyaButtonPadEvents.Button1SingleClick))
            {
                ToggleMonitorLed();
                return;
            }

            if (message.Action.Equals(TuyaButtonPadEvents.Button1DoubleClick))
            {
                return;
            }

            if (message.Action.Equals(TuyaButtonPadEvents.Button2SingleClick))
            {
                ToggleCockroachRepeller();
                //ToggleSideRoomGreenhouseLight();
                return;
            }

            if (message.Action.Equals(TuyaButtonPadEvents.Button2DoubleClick))
            {
                return;
            }

            if (message.Action.Equals(TuyaButtonPadEvents.Button3SingleClick))
            {
                IrrigationStation2StartPump(1, 1);
                return;
            }

            if (message.Action.Equals(TuyaButtonPadEvents.Button3DoubleClick))
            {
                IrrigationStation2StartPump(1, 10);
                return;
            }

            if (message.Action.Equals(TuyaButtonPadEvents.Button4SingleClick))
            {
                IrrigationStation2StartPump(2, 1);
                return;
            }

            if (message.Action.Equals(TuyaButtonPadEvents.Button4DoubleClick))
            {
                IrrigationStation2StartPump(2, 10);
                return;
            }
        }
        private void OnButtonPad12MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            var jsonStr = Encoding.UTF8.GetString(eventArgs.Message);
            var message = JsonConvert.DeserializeObject<ModkamButtonPadMsg>(jsonStr);

            if (message.Action == null)      // System message
                return;

            if (message.Action.Equals(ModkamButtonPadEvents.Button1SingleClick))
            {
                IrrigationStation2StartPump(1, 10);
                //ToggleMonitorLed();
                return;
            }

            if (message.Action.Equals(ModkamButtonPadEvents.Button2SingleClick))
            {
                IrrigationStation2StartPump(2, 10);
                //IrrigationStation1StartPump(1);
                return;
            }

            if (message.Action.Equals(ModkamButtonPadEvents.Button3SingleClick))
            {
                //IrrigationStation1StartPump(2);
                //_easyEspClient.Unit2PlaySoundAsync("d=10,o=6,b=180,c,e,g").Wait();
                return;
            }

            if (message.Action.Equals(ModkamButtonPadEvents.Button4SingleClick))
            {
                //IrrigationStation1StartPump(3);
                return;
            }
        }
        private void OnSideRoomKaktusLightButtonMessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
            var message = JsonConvert.DeserializeObject<AquaraButtonMsg>(jsonMessage);

            if (message.Action == null)      // System message
                return;

            if (message.Action.Equals(AquaraButtonEvents.SingleClick))
                ToggleSideRoomGreenhouseLight();
        }
        private void OnButton1MessageReceived(Object sender, MqttMsgPublishEventArgs eventArgs)
        {
            var jsonMessage = Encoding.UTF8.GetString(eventArgs.Message);
            var message = JsonConvert.DeserializeObject<AquaraButtonMsg>(jsonMessage);

            if (message.Action == null)      // System message
                return;

            if (message.Action.Equals(AquaraButtonEvents.SingleClick))
            {
                //ToggleMonitorLed();
                ToggleMiddleRoomGreenhouseLight();
            }
        }
    }
}