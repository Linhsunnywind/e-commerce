using E_commerce.Data;
using E_commerce.DTOs.SupportRequest;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class SupportService : ISupportService
    {
        private readonly AppDbContext _context;

        public SupportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SupportRequestResponseDto> CreateTicketAsync(Guid userId, CreateSupportRequestDto dto)
        {
            var ticket = new SupportRequest
            {
                UserId = userId,
                Subject = dto.Subject,
                Message = dto.Message,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            _context.SupportRequests.Add(ticket);
            await _context.SaveChangesAsync();

            // Fetch user to map name in response
            var user = await _context.Users.FindAsync(userId);

            return new SupportRequestResponseDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                CustomerName = user?.FullName ?? "Unknown",
                Subject = ticket.Subject,
                Message = ticket.Message,
                Status = ticket.Status,
                CreatedDate = ticket.CreatedDate
            };
        }

        public async Task<IEnumerable<SupportRequestResponseDto>> GetTicketsByUserIdAsync(Guid userId)
        {
            return await _context.SupportRequests
                .Include(s => s.User)
                .Where(s => s.UserId == userId)
                .Select(s => new SupportRequestResponseDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    CustomerName = s.User.FullName,
                    Subject = s.Subject,
                    Message = s.Message,
                    Status = s.Status,
                    CreatedDate = s.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<SupportRequestResponseDto> UpdateTicketStatusAsync(Guid ticketId, string status)
        {
            var validStatuses = new[] { "Pending", "InProgress", "Resolved" };
            if (!validStatuses.Contains(status))
                throw new ArgumentException($"Invalid status: {status}");

            var ticket = await _context.SupportRequests
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == ticketId)
                ?? throw new KeyNotFoundException("Ticket not found");

            ticket.Status = status;
            await _context.SaveChangesAsync();

            return new SupportRequestResponseDto
            {
                Id = ticket.Id,
                UserId = ticket.UserId,
                CustomerName = ticket.User?.FullName ?? "Unknown",
                Subject = ticket.Subject,
                Message = ticket.Message,
                Status = ticket.Status,
                CreatedDate = ticket.CreatedDate
            };
        }

        public async Task<IEnumerable<SupportRequestResponseDto>> GetAllTicketsAsync(string? status)
        {
            var query = _context.SupportRequests.Include(s => s.User).AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status.ToLower() == status.ToLower());
            }

            return await query
                .Select(s => new SupportRequestResponseDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    CustomerName = s.User.FullName,
                    Subject = s.Subject,
                    Message = s.Message,
                    Status = s.Status,
                    CreatedDate = s.CreatedDate
                })
                .ToListAsync();
        }
    }
}