using Microsoft.AspNetCore.Components;
using System.Timers;
using Timer = System.Timers.Timer;

namespace IotHub.Ui.Pages
{
    [Route("autoCounter")]
    public partial class AutoCounter
    {
        private Int32 _currentCount;
        private Timer _timer;


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        protected override void OnInitialized()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += Tick;
            _timer.Start();
        }
        private void Tick(Object sender, ElapsedEventArgs e)
        {
            _currentCount++;
            StateHasChanged();
        }
    }
}