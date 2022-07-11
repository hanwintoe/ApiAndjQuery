using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiAndjQuery.Data;
using ApiAndjQuery.Dto;
using ApiAndjQuery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAndjQuery.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static ReservationDto ReservationToDto(Reservation reservation) => new()
        {
            Id = reservation.Id,
            Name = reservation.Name,
            StartLocation = reservation.StartLocation,
            EndLocation = reservation.EndLocation,
        };


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            return await _context.Reservations
                .Select(x => ReservationToDto(x))
                .ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            var rev = await _context.Reservations.FindAsync(id);
            if (rev == null)
                return NotFound();
            return ReservationToDto(rev);

        }

        [HttpPost]
        public async Task<ActionResult<ReservationDto>> CreateReservation(ReservationDto reservationDto)
        {
            var reservation = new Reservation
            {
                Name = reservationDto.Name,
                StartLocation = reservationDto.StartLocation,
                EndLocation = reservationDto.EndLocation,
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, ReservationToDto(reservation));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationDto reservationDto)
        {
            if (id != reservationDto.Id)
                return BadRequest();

            Reservation reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.Name = reservationDto.Name;
            reservation.StartLocation = reservationDto.StartLocation;
            reservation.EndLocation = reservationDto.EndLocation;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ReservationExists(id))
            {

                return NotFound();
            }
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var rev = await _context.Reservations.FindAsync(id);
            if (rev == null)
                return NotFound();

            _context.Reservations.Remove(rev);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool ReservationExists(int id) => _context.Reservations.Any(e => e.Id == id);
    }

}
