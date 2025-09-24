namespace AuthService.Common.Dtos
{
    public record RegisterDto(
        string Email,
        string Password,
        string FullName,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string Address,
        string City,
        string State,
        string Country,
        string ZipCode,
        string ProfilePictureUrl
    );

}
