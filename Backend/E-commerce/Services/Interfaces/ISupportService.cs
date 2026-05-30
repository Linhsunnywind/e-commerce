using E_commerce.DTOs.SupportRequest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_commerce.Services.Interfaces
{
    public interface ISupportService
    {
        Task<SupportRequestResponseDto> CreateTicketAsync(Guid userId, CreateSupportRequestDto dto);
        Task<IEnumerable<SupportRequestResponseDto>> GetTicketsByUserIdAsync(Guid userId);
        Task<IEnumerable<SupportRequestResponseDto>> GetAllTicketsAsync(string? status);
        Task<SupportRequestResponseDto> UpdateTicketStatusAsync(Guid ticketId, string status);
    }
}