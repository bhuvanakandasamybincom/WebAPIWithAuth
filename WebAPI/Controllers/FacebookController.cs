using DocumentFormat.OpenXml.Spreadsheet;

namespace BoardCasterWebAPI.Controllers
{
    //Adding role and policy checks
    //[Authorize(Policy = "AdminPolicy")]
    [Authorize(Roles ="User")]
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]    
    //[Authorize(Roles = "Admin")]
    public class FacebookController(IConfiguration configuration) : ControllerBase
    {
        HttpClient _httpClient = new HttpClient();
        string PagePostUrl = "https://graph.facebook.com/v22.0/{0}/feed?message={1}&access_token={2}";
        //Primary constructor implementation
        private readonly string _facebookAccessToken = configuration["Facebook:PageAccessToken"];
        private readonly string _pageId = configuration["Facebook:PageId"];

        /// <summary>
        /// Post Message to facebook page
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("PostMessage")]
        [ProducesResponseType(typeof(APIResult<string>),200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[AllowAnonymous]
        public async Task<IActionResult> PostMessage(string PageAccessToken, [FromBody] FacebookUser facebookUser)
        {
            APIResult<string> endPointResult = new();
            if (string.IsNullOrEmpty(PageAccessToken))
                throw new ArgumentNullException(nameof(PageAccessToken));
            if (string.IsNullOrEmpty(facebookUser.message))
                throw new ArgumentNullException(nameof(facebookUser.message));
            string FormattedUrl= string.Format(PagePostUrl, _pageId, facebookUser.message, PageAccessToken);

            var result= await _httpClient.PostAsync(FormattedUrl,null);
                if (result.IsSuccessStatusCode)//StatusCode == HttpStatusCode.OK)
                {
                //var BodyContent1 = await result.Content.ReadFromJsonAsync();
                var BodyContent = await result.Content.ReadAsStringAsync();
                    endPointResult.Result = BodyContent;
                    return Ok(endPointResult);
                }
                else
                {
                    endPointResult.ResponseCode = Convert.ToInt32(result.StatusCode);
                    endPointResult.Message = "Error : " + result.ReasonPhrase;
                    throw new HttpRequestException(
               $"Invalid status code in the HttpResponseMessage: {endPointResult.ResponseCode.ToString()}.");
                     
                }

}
        private string PostMessage()
        {
            return "";
        }
    }
}
