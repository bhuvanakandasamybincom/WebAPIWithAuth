using System.ComponentModel;

namespace BoardCasterWebAPI.Model
{
    public class APIResult<T>()
    {
        

        [DefaultValue(200)]
        public int ResponseCode { get; set; } 
        [DefaultValue("")]
        public T? Result { get; set; } 
        
        [DefaultValue("Success")]
        public string? Message { get; set; }
        
    }
}
