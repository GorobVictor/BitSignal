namespace WebApi.Model.Account;

public class LoginResponse
{
    public LoginResponse(string token)
    {
        this.Token = token;
    }

    public string Token { get; set; }
}