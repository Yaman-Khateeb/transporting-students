﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using BL;
using DAL;

namespace API.Controllers
{
    [Route("api/EventRides")]
    [ApiController]
    public class EventRidesController : ControllerBase
    {
        /// <summary>
        /// Creates a new event ride.
        /// </summary>
        /// <param name="eventRide"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateEventRide")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EventRideDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<EventRideDTO> CreateEventRide([FromBody] EventRideDTO eventRide)
        {
            if (eventRide == null || eventRide.EventID <= 0 || eventRide.DriverID <= 0 ||
                eventRide.PassengerID <= 0 || eventRide.PassengerRegistrationID <= 0 || eventRide.DriverRegistrationID <= 0)
            {
                return BadRequest("Invalid event ride data.");
            }
            try
            {
                bool created = EventRides.CreateNewEventRide(eventRide);
                if (created)
                {
                    return StatusCode(StatusCodes.Status201Created, eventRide);
                }
                return BadRequest("Failed to create event ride.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing event ride.
        /// </summary>
        /// <param name="eventRide"></param>
        /// <returns></returns>
        [HttpPut(Name = "UpdateEventRide")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateEventRide([FromBody] EventRideDTO eventRide)
        {
            if (eventRide == null || eventRide.ID <= 0 || eventRide.EventID <= 0 || eventRide.DriverID <= 0 ||
                eventRide.PassengerID <= 0 || eventRide.PassengerRegistrationID <= 0 || eventRide.DriverRegistrationID <= 0)
            {
                return BadRequest("Invalid event ride data.");
            }
            try
            {
                bool updated = EventRides.UpdateEventRide(eventRide);
                if (updated)
                {
                    return Ok("Event ride updated successfully.");
                }
                return BadRequest("Failed to update event ride.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Gets an event ride by event ID and driver ID.
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="driverID"></param>
        /// <returns></returns>
        [HttpGet("{eventRideID}", Name = "GetEventRideByID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventRideDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<EventRideDTO> GetEventRideByID(int eventRideID)
        {
            if (eventRideID <= 0)
            {
                return BadRequest("Invalid eventRideID.");
            }
            try
            {
                var eventRide = EventRides.GetEventRideByID(eventRideID);
                return Ok(eventRide);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Gets all event rides by event ID and driver ID.
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="driverID"></param>
        /// <returns></returns>
        [HttpGet("by-event-driver/{eventID}/{driverID}", Name = "GetEventRidesByEventIDAndDriverID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EventRideDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<EventRideDTO>> GetEventRidesByEventIDAndDriverID(int eventID, int driverID)
        {
            if (eventID <= 0 || driverID <= 0)
            {
                return BadRequest("Invalid event or driver ID.");
            }
            var eventRides = EventRides.GetEventRidesByEventIDAndDriverID(eventID, driverID);
            return Ok(eventRides);
        }

        [HttpGet("passengers/{eventID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<PassengerDTO>> GetAllPassengersForEvent(int eventID)
        {
            try
            {
                var passengers = EventRides.GetAllPassengersForEvent(eventID);
                return Ok(passengers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("passengers/no-car/{eventID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<PassengerDTO>> GetPassengersForEventWithNoCarAssigned(int eventID)
        {
            try
            {
                var passengers = EventRides.GetPassengersForEventWithNoCarAssigned(eventID);
                return Ok(passengers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("Drivers/{eventID}", Name = "GetAllDriversForEvent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<DriverDTO>> GetAllDriversForEvent(int eventID)
        {
            var drivers = EventRides.GetAllDriversForEvent(eventID);
            if (drivers.Count == 0)
                return NotFound($"No drivers found for event ID {eventID}!");
            return Ok(drivers);
        }

        [HttpGet("DriversWithAvailableCapacity/{eventID}", Name = "GetDriversWithAvailableCapacityForEvent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<DriverDTO>> GetDriversWithAvailableCapacityForEvent(int eventID)
        {
            var drivers = EventRides.GetDriversWithAvailableCapacityForEvent(eventID);
            if (drivers.Count == 0)
                return NotFound($"No drivers with available capacity found for event ID {eventID}!");
            return Ok(drivers);
        }


    }
}
