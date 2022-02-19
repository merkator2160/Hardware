using IotHub.Common.BasicMediator.Interfaces;
using System;

namespace Sandbox.Units.Mediator
{
    internal class Subscriber
    {
        public Subscriber(IMessenger messenger)
        {
            messenger.Register<String>(this, Console.WriteLine);
        }
    }
}