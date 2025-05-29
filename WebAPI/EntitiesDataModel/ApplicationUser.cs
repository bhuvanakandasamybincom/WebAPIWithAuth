using System.Security.Cryptography.X509Certificates;

namespace WebAPI.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="Password"></param>
    
    // "record" is for data model, primary role is to store data
    public record ApplicationUser(
      string UserId,
      string Password  
        );
    // "record" is for data model, primary role is to store data
    public record RegisterUser(
      string UserId,
      string Password
        )
    {
        public required string[] PhoneNumber { get; init; }
    }
}
