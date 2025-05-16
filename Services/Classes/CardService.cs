using System.Security.Claims;
using AutoMapper;
using ControllerFirst.Contexts;
using ControllerFirst.Data.Models;
using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using ControllerFirst.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControllerFirst.Services.Classes;

public class CardService : ICardService
{
    private readonly AuthContext _context;
    private readonly EncryptionService _encryptionService;
    private readonly IMapper _mapper;

    public CardService(AuthContext context, EncryptionService encryptionService, IMapper mapper)
    {
        _context = context;
        _encryptionService = encryptionService;
        _mapper = mapper;
    }
    
    private CardType DetectCardType(string number)
    {
        if (number.StartsWith("4"))
            return CardType.Visa;
        if (number.StartsWith("5"))
            return CardType.MasterCard;
        if (number.StartsWith("3"))
            return CardType.AmericanExpress;
        
        throw new ArgumentException("Unknown card type");
    }
    

    public async Task<CardResponse> CreateCardAsync(ClaimsPrincipal userClaimsPrincipal,CreateCardRequest request)
    {
        var userId = userClaimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user = await _context.Users.FindAsync(userId);
        
        var card = _mapper.Map<UserCard>(request);
        
        card.UserId = Guid.Parse(user.Id.ToString());
        card.CardNumberEncrypted = _encryptionService.Encrypt(request.number);
        card.CVVEncrypted = _encryptionService.Encrypt(request.cvv);
        card.CardType = DetectCardType(request.number);

        _context.UserCards.Add(card);
        await _context.SaveChangesAsync();
        
        return new CardResponse
        {
            Id = card.Id.ToString(),
            CardHolder = card.CardHolder,
            IsDefault = card.IsDefault,
            CardType = card.CardType.ToString(), 
            Last4 = request.number[^4..]
        };
    }

    public async Task<CardResponse> UpdateCardAsync(ClaimsPrincipal userClaimsPrincipal, UpdateCardRequest request)
    {
        var userId = userClaimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user = await _context.Users.FindAsync(userId);

        var card = await _context.UserCards.FirstOrDefaultAsync(c => c.Id == Guid.Parse(request.cardId) && c.UserId == Guid.Parse(user.Id.ToString()));
        
        if (card == null)
        {
            throw new Exception("Card not found");
        }

        _mapper.Map<UpdateCardRequest, UserCard>(request, card);
        
        card.CardNumberEncrypted = _encryptionService.Encrypt(request.number);
        card.CVVEncrypted = _encryptionService.Encrypt(request.cvv);
        card.CardType = DetectCardType(request.number);
        card.IsDefault = request.isDefault;
        
        await _context.SaveChangesAsync();

        return new CardResponse
        {
            Id = card.Id.ToString(),
            CardHolder = card.CardHolder,
            IsDefault = card.IsDefault,
            CardType = card.CardType.ToString(), 
            Last4 = request.number[^4..]
        };
    }

    public async Task<CardResponse> DeleteCardAsync(ClaimsPrincipal userClaimsPrincipal, DeleteCardRequest request)
    {
        var userId = userClaimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user = await _context.Users.FindAsync(userId);

        
        var card = await _context.UserCards.FirstOrDefaultAsync(c => c.Id == Guid.Parse(request.cardId) && c.UserId == Guid.Parse(user.Id.ToString()));

        if (card == null)
        {
            throw new Exception("Card not found");
        }

        _context.UserCards.Remove(card);
        await _context.SaveChangesAsync();

        var decryptedNumber = _encryptionService.Decrypt(card.CardNumberEncrypted);
        
        return new CardResponse
        {
            Id = card.Id.ToString(),
            CardHolder = card.CardHolder,
            IsDefault = card.IsDefault,
            CardType = card.CardType.ToString(), 
            Last4 = decryptedNumber[^4..]
        };
    }

    public async Task<CardResponse> GetCardAsync(string cardId)
    {
        throw new NotImplementedException();
    }
}