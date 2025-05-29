namespace BoardCasterWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    //Adding role and policy checks
    [Authorize(Policy = "RequireAdministratorRole")]
    [Authorize(Roles = "Administrator")]
    public class FacebookController(IConfiguration configuration) : ControllerBase
    {
        HttpClient _httpClient = new HttpClient();
        string PagePostUrl = "https://graph.facebook.com/v22.0/{0}/feed?message={1}&access_token={2}";
        //Primary constructor implementation
        private readonly string _facebookAccessToken = configuration["Facebook:AccessToken"];
        private readonly string _pageId = configuration["Facebook:PageId"];

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("PostMessage")]
        [ProducesResponseType(typeof(APIResult<string>),200)]
        [AllowAnonymous]
        public async Task<IActionResult> PostMessage(string accessToken,[FromBody] FacebookUser facebookUser)
        {
            APIResult<string> endPointResult = new();
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException(nameof(accessToken));
            if (string.IsNullOrEmpty(facebookUser.message))
                throw new ArgumentNullException(nameof(facebookUser.message));


            string FormattedUrl= string.Format(PagePostUrl, "651558531371342", facebookUser.message, accessToken);

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
            
            //}
            //catch (HttpRequestException ex)
            //{
            //    endPointResult.ResponseCode = 500;
            //    endPointResult.Message = $"An error occurred: {ex.Message}";
            //    return BadRequest(endPointResult);
            //}

}
        private string PostMessage()
        {
            return "";
        }
    }
}
