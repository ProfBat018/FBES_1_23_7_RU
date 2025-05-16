namespace ControllerFirst.DTO.Responses;

public record AddressResponse(string country, string city,
    string street, string zipCode, bool isDefault);