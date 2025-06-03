using System.Security.Cryptography.X509Certificates;

namespace WebAPI.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="Password"></param>

    //// "record" is for data model, primary role is to store data
    //public record ApplicationUser(
    //  string UserName,
    //  string Password,
    //  string Email
    //    );
    //// "record" is for data model, primary role is to store data
    //public record RegisterUser(
    //  string UserName,
    //  string Password,
    //  string Email
    //    )
    //{
    //    //public required string[] PhoneNumber { get; init; } = [{"4665656565"}]
    //}
    /// <summary>
    /// ApplicationUser class will inherit the class IdentityUser so that we can add a field Name to User's Identity table in database
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
       public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        public string SecurityStamp {  get; set; }
        /// <summary>
        /// Gets or sets the name of the user. Maximum length is 30 characters.
        /// </summary>
        [MaxLength(30)]
        public string Name { get; set; }

    }
}
