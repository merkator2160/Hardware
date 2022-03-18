using Microsoft.AspNetCore.Components;
using System;

namespace IotHub.Ui.Pages
{
    [Route("counter")]
    public partial class Counter
    {
        private Int32 _currentCount = 0;


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private void IncrementCount()
        {
            _currentCount++;
        }
    }
}