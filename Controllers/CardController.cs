using ControllerFirst.DTO.Requests;
using ControllerFirst.DTO.Responses;
using ControllerFirst.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControllerFirst.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }


    [HttpGet("All")]
    public async Task<IActionResult> GetCardsAsync()
    {
        throw new NotImplementedException();
    }


    [HttpPost("Add")]
    public async Task<IActionResult> CreateCardAsync([FromBody] CreateCardRequest request)
    {
        var res = await _cardService.CreateCardAsync(User, request);
        
        return Ok(Result<CardResponse>.Success(res, "Card created successfully"));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCardAsync([FromBody] DeleteCardRequest request)
    {
        var res = await _cardService.DeleteCardAsync(User, request);
        
        return Ok(Result<CardResponse>.Success(res, "Card deleted successfully"));
    }

    [HttpPost("Update")]
    public async Task<IActionResult> UpdateCardAsync([FromBody] UpdateCardRequest request)
    {
        var res = await _cardService.UpdateCardAsync(User, request);
        
        return Ok(Result<CardResponse>.Success(res, "Card updated successfully"));
    }
}