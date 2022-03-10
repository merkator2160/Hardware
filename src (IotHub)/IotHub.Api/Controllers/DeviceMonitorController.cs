using AutoMapper;
using IotHub.Api.Services.Interfaces;
using IotHub.Contracts.Models.Api.DeviceMonitor;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IotHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DeviceMonitorController : ControllerBase
    {
        private readonly IDeviceMonitor _deviceMonitor;
        private readonly IMapper _mapper;


        public DeviceMonitorController(IDeviceMonitor deviceMonitor, IMapper mapper)
        {
            _deviceMonitor = deviceMonitor;
            _mapper = mapper;
        }


        // ACTIONS ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns information about MQTT devices under tracking
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(TrackingDeviceAm[]), 200)]
        [ProducesResponseType(typeof(String), 460)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult GetAllDeviceInfo()
        {
            return Ok(_mapper.Map<TrackingDeviceAm[]>(_deviceMonitor.GetAllDeviceInfo()));
        }

        /// <summary>
        /// Returns information about unavailable MQTT devices
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(TrackingDeviceAm[]), 200)]
        [ProducesResponseType(typeof(String), 460)]
        [ProducesResponseType(typeof(String), 500)]
        public IActionResult GetUnavailableDeviceInfo()
        {
            return Ok(_mapper.Map<TrackingDeviceAm[]>(_deviceMonitor.GetAllUnavailableDeviceInfo()));
        }
    }
}
