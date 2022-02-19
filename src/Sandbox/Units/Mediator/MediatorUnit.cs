using IotHub.Common.BasicMediator;
using System;

namespace Sandbox.Units.Mediator
{
    internal static class MediatorUnit
    {
        public static void Run()
        {
            var messenger = new Messenger();
            var subscriber = new Subscriber(messenger);

            messenger.Send("Hello world!");

            Console.ReadKey();
        }
    }
}