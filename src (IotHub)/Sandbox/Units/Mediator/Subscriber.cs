using IotHub.Common.BasicMediator.Interfaces;

namespace Sandbox.Units.Mediator
{
    public class Subscriber
    {
        public Subscriber(IMessenger messenger)
        {
            messenger.Register<String>(this, WriteMessage);
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private void WriteMessage(String str)
        {
            Console.WriteLine(str);
        }
    }
}