using System.Security.Claims;
using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;

namespace ControllerFirst.Services.Interfaces;

public interface ICardService
{
    public Task<CardResponse> CreateCardAsync(ClaimsPrincipal userClaimsPrincipal, CreateCardRequest request);
    public Task<CardResponse> UpdateCardAsync (ClaimsPrincipal userClaimsPrincipal,UpdateCardRequest request);
    public Task<CardResponse> DeleteCardAsync(ClaimsPrincipal userClaimsPrincipal,DeleteCardRequest request);
    
    public Task<CardResponse> GetCardAsync(string cardId);
    
    
    
}