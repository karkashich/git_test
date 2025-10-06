namespace SeleniumTests;
public sealed class CredentialsOptions
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    
}

public sealed class AppUrlOptions
{
    public required string BaseUrl { get; init; }
}