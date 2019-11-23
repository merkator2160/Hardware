using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttSandbox.Units
{
	internal static class MqttUnit
	{
		private const String _brokerIpAddress = "192.168.1.11";


		public static void Run()
		{
			var client = new MqttClient(_brokerIpAddress);

			client.MqttMsgPublishReceived += OnMqttMsgPublishReceived;

			var clientId = Guid.NewGuid().ToString();
			client.Connect(clientId);

			//client.Subscribe(new String[] { "/home/temperature" }, new Byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
			client.Subscribe(new String[] { "/test" }, new Byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

			Console.ReadKey();

			client.Disconnect();
		}
		private static void OnMqttMsgPublishReceived(Object sender, MqttMsgPublishEventArgs e)
		{
			Console.WriteLine(Encoding.UTF8.GetString(e.Message));
		}

		private static void Publish(MqttClient client, String value)
		{
			var strValue = Convert.ToString(value);

			client.Publish("/test", Encoding.UTF8.GetBytes(strValue), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}
	}
}