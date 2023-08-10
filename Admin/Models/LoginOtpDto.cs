namespace Client.Models.Dtos
{
    public class LoginOtpDto
    {
        public string PhoneNumber { get; set; }
        public string ClientId { get; set; }
        public int Code { get; set; }
        public string Password { get; set; }
        public string? OTP { get; set; }
    }
}
