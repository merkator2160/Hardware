#include <EspMQTTClient.h>


EspMQTTClient client(
    "2160",
    "zxcvbnm,",
    "192.168.1.11",
    "TestClient",     // Client name that uniquely identify your device
    1883              // The MQTT port, default to 1883. this line can be omitted
);


void setup()
{
    pinMode(LED_BUILTIN, OUTPUT);
	
    Serial.begin(115200);

    // Optionnal functionnalities of EspMQTTClient : 
    client.enableDebuggingMessages(); // Enable debugging messages sent to serial output
    client.enableHTTPWebUpdater(); // Enable the web updater. User and password default to values of MQTTUsername and MQTTPassword. These can be overrited with enableHTTPWebUpdater("user", "password").
    client.enableLastWillMessage("TestClient/lastwill", "I am going offline");  // You can activate the retain flag by setting the third parameter to true
}
void loop()
{
    client.loop();
}


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////////
void onConnectionEstablished()
{
    client.subscribe("mytopic/led", [](const String& payload)
    {
        Serial.println(payload);
        digitalWrite(LED_BUILTIN, payload.toInt());
    });
	
    client.subscribe("mytopic/test", [](const String& payload) 
    {
        Serial.println(payload);
    });

    client.subscribe("mytopic/wildcardtest/#", [](const String& topic, const String& payload) 
    {
        Serial.println("(From wildcard) topic: " + topic + ", payload: " + payload);
    });

    // Publish a message to "mytopic/test"
    client.publish("mytopic/test", "This is a message"); // You can activate the retain flag by setting the third parameter to true

    // Execute delayed instructions
    client.executeDelayed(5 * 1000, []() 
    {
        client.publish("mytopic/wildcardtest/test123", "This is a message sent 5 seconds later");
    });
}