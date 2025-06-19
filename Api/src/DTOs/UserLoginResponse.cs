namespace InvestSync.Api.src.DTOs
{
    public class UserLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserResponse User { get; set; } = new();
    }
}