using Microsoft.SPOT.Hardware;
using Sandbox.Units.StepEngineDir.Enums;
using System;
using System.Threading;

namespace Sandbox.Units.StepEngineDir
{
    public class Stepper : IDisposable
    {
        private readonly OutputPort[] _stepEngineStatePorts;


        public Stepper(Cpu.Pin pin0, Cpu.Pin pin1, Cpu.Pin pin2, Cpu.Pin pin3)
        {
            _stepEngineStatePorts = new[]
            {
                new OutputPort(pin0, true),
                new OutputPort(pin1, false),
                new OutputPort(pin2, false),
                new OutputPort(pin3, false),
            };
        }

        // PROPERTIES /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Number of steps per cycle. Full step mode
        /// </summary>
        public const Int32 StepsPerRevolutionSinglePrecisione = 2048;

        /// <summary>
        /// Number of steps per cycle. Half step mode
        /// </summary>
        public const Int32 StepsPerRevolutionDoublePrecisione = 4096;


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void Move(Direction direction, Int32 numberOfSteps, Int32 stepDelay)
        {
            for (var i = 0; i < numberOfSteps; i++)
            {
                MakeStep(direction, stepDelay);
            }
        }
        public void MakeStep(Direction direction, Int32 stepDelay)
        {
            if (direction == Direction.Forward)
            {
                MakeOneStepForward(_stepEngineStatePorts);
            }
            else
            {
                MakeOneStepBackward(_stepEngineStatePorts);
            }
            Thread.Sleep(stepDelay);
        }
        private static void MakeOneStepForward(OutputPort[] shiftPorts)
        {
            var lastBit = shiftPorts[shiftPorts.Length - 1].Read();
            for (var i = shiftPorts.Length - 1; i > 0; i--)
            {
                shiftPorts[i].Write(shiftPorts[i - 1].Read());
            }
            shiftPorts[0].Write(lastBit);
        }
        private static void MakeOneStepBackward(OutputPort[] shiftPorts)
        {
            var firstBit = shiftPorts[0].Read();
            for (var i = 0; i < shiftPorts.Length - 1; i++)
            {
                shiftPorts[i].Write(shiftPorts[i + 1].Read());
            }
            shiftPorts[shiftPorts.Length - 1].Write(firstBit);
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            foreach (var x in _stepEngineStatePorts)
            {
                x.Dispose();
            }
        }
    }
}