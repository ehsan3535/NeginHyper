namespace Entities.TempUser
{
    public class TempUser : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Otp { get; set; }
    }
}
