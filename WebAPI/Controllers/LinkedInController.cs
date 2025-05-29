using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using BoardCasterWebAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoardCasterWebAPI.Model;


namespace BoardCasterWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    public class LinkedInController :  ILinkedInManager
    {
        const string LinkedInUserUrl = "https://api.linkedin.com/v2/userinfo";
        const string LinkedInPostUrl = "https://api.linkedin.com/v2/ugcPosts";
        private const string LinkedInAuthorUrn = "urn:li:person:{0}";
        private const string LinkedInAssetUrl = "https://api.linkedin.com/v2/assets?action=registerUpload";
        HttpClient _httpClient = new HttpClient();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>

        [HttpGet]
        [Route("GetLinkedInUser")]
        [AllowAnonymous]
        public async Task<LinkedInUser> GetMyLinkedInUserProfile(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            var profile= await ExecuteGetAsync<LinkedInUser>(LinkedInUserUrl, accessToken);

           //var str= await PostShareText(accessToken, profile.id,"Second LinkedIn post");
            return profile;
        }

        [HttpGet]
        [Route("ExecuteGetAsync")]
        private async Task<T> ExecuteGetAsync<T>(string url, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(
                    $"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");

            // Parse the Results
            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var results = JsonSerializer.Deserialize<T>(content, options);

            if (results == null)
            {
                throw new HttpRequestException(
                    $"Unable to deserialize the response from the HttpResponseMessage: {content}.");
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="authorId"></param>
        /// <param name="postText"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>

        [HttpPost]
        [Route("PostShareText")]
        public async Task<string> PostShareText(string accessToken, string authorId, string postText)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (string.IsNullOrEmpty(authorId))
            {
                throw new ArgumentNullException(nameof(authorId));
            }
            if (string.IsNullOrEmpty(postText))
            {
                throw new ArgumentNullException(nameof(postText));
            }

            var shareRequest = new ShareRequest
            {
                Author = string.Format(LinkedInAuthorUrn, authorId),
                //LifecycleState,
                Visibility = new Visibility
                {
                    VisibilityEnum= VisibilityEnum.ConnectionsOnly
                },//$"""com.linkedin.ugc.MemberNetworkVisibility": "PUBLIC""",
                  
                SpecificContent = new SpecificContent
                {
                    ShareContent = new ShareContent
                    {
                        ShareCommentary = new TextProperties()
                        {
                            Text = postText
                        },
                        ShareMediaCategoryEnum = ShareMediaCategoryEnum.None
                    }
                }
            };
            var linkedInResponse = await CallPostShareUrl(accessToken, shareRequest);
            if (linkedInResponse is { IsSuccess: true, Id: not null })
            {
                return linkedInResponse.Id;
            }
            throw new HttpRequestException($"Failed to post status update to LinkedIn: LinkedIn Status Code: '{linkedInResponse.ServiceErrorCode}', LinkedIn Message: '{linkedInResponse.Message}'");
        }

        [HttpPost]
        [Route("CallPostShareUrl")]
        private async Task<ShareResponse> CallPostShareUrl(string accessToken, ShareRequest shareRequest)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, LinkedInPostUrl);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            //requestMessage.Headers.Add("X-Restli-Protocol-Version", "2.0.0");

            JsonSerializerOptions jsonSerializationOptions = new(JsonSerializerDefaults.Web)
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            var jsonRequest = JsonSerializer.Serialize(shareRequest, jsonSerializationOptions);
            var jsonContent = new StringContent(jsonRequest, null, "application/json");
            requestMessage.Content = jsonContent;

            var response = await _httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var linkedInResponse = JsonSerializer.Deserialize<ShareResponse>(content, options);

            if (linkedInResponse == null)
            {
                // TODO: Custom Exception
                throw new HttpRequestException(
                    $"Unable to deserialize the response from the HttpResponseMessage: {content}.");
            }

            return linkedInResponse;
        }

        [HttpPost]
        [Route("PostShareTextAndImage")]
        public async Task<string> PostShareTextAndImage(string accessToken, string authorId, string postText, byte[] image, string? imageTitle = null, string? imageDescription = null)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (string.IsNullOrEmpty(authorId))
            {
                throw new ArgumentNullException(nameof(authorId));
            }
            if (string.IsNullOrEmpty(postText))
            {
                throw new ArgumentNullException(nameof(postText));
            }
            // Call the Register Image endpoint to get the Asset URN
            //Get the linkedIn URL to upload image
            var uploadResponse = await GetUploadResponse(accessToken, authorId);

            // Upload the image
            var uploadUrl = uploadResponse.Value.UploadMechanism.MediaUploadHttpRequest.UploadUrl;
            var wasFileUploadSuccessful = await UploadImage(accessToken, uploadUrl, image);

            if (!wasFileUploadSuccessful)
            {
                throw new ApplicationException("Failed to upload image to LinkedIn");
            }

            // Send the image via PostShare
            var shareRequest = new ShareRequest
            {
                Author = string.Format(LinkedInAuthorUrn, authorId),
                Visibility = new Visibility { VisibilityEnum = VisibilityEnum.ConnectionsOnly },
                SpecificContent = new SpecificContent
                {
                    ShareContent = new ShareContent
                    {
                        ShareCommentary = new TextProperties()
                        {
                            Text = postText.ToString()
                        },
                        ShareMediaCategoryEnum = ShareMediaCategoryEnum.Image
                    }
                }
            };

            var media = new Media { MediaUrn = uploadResponse.Value.Asset };

            if (!string.IsNullOrEmpty(imageDescription))
            {
                media.Description = new TextProperties { Text = imageDescription };
            }
            if (!string.IsNullOrEmpty(imageTitle))
            {
                media.Title = new TextProperties { Text = imageTitle };
            }
            shareRequest.SpecificContent.ShareContent.Media = new[] { media };

            var linkedInResponse = await CallPostShareUrl(accessToken, shareRequest);
            if (linkedInResponse is { IsSuccess: true, Id: not null })
            {
                return linkedInResponse.Id;
            }
            throw new HttpRequestException(BuildLinkedInResponseErrorMessage(linkedInResponse));
        }
        private async Task<UploadRegistrationResponse> GetUploadResponse(string accessToken, string authorId)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (string.IsNullOrEmpty(authorId))
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            var uploadRequest = new UploadRegistrationRequest
            {
                RegisterUploadRequest = new RegisterUploadRequest
                {
                    Owner = string.Format(LinkedInAuthorUrn, authorId)
                }
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, LinkedInAssetUrl);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            requestMessage.Headers.Add("X-Restli-Protocol-Version", "2.0.0");

            JsonSerializerOptions jsonSerializationOptions = new(JsonSerializerDefaults.Web)
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var jsonRequest = JsonSerializer.Serialize(uploadRequest, jsonSerializationOptions);
            var jsonContent = new StringContent(jsonRequest, null, "application/json");
            requestMessage.Content = jsonContent;

            var response = await _httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var uploadResponse = JsonSerializer.Deserialize<UploadRegistrationResponse>(content, options);

            if (uploadResponse == null)
            {
                // TODO: Custom Exception
                throw new ApplicationException("Could not deserialize the response from LinkedIn");
            }
            return uploadResponse;
        }
        private async Task<bool> UploadImage(string accessToken, string uploadUrl, byte[] image)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            if (string.IsNullOrEmpty(uploadUrl))
            {
                throw new ArgumentNullException(nameof(uploadUrl));
            }
            if (image == null || image.Length == 0)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, uploadUrl);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            requestMessage.Headers.Add("X-Restli-Protocol-Version", "2.0.0");

            requestMessage.Content = new ByteArrayContent(image);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new HttpRequestException(
                    $"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
            }

            return true;
        }

        private string BuildLinkedInResponseErrorMessage(ShareResponse shareResponse)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Failed to post status update to LinkedIn. ");
            stringBuilder.AppendLine($"LinkedIn Status Code: '{shareResponse.Status}', ");
            stringBuilder.AppendLine($"LinkedIn Service Error Code: '{shareResponse.ServiceErrorCode}', ");
            stringBuilder.AppendLine($"LinkedIn Message: '{shareResponse.Message}'. ");
            //return stringBuilder.ToString();
            return stringBuilder.ToString();
        }

    }
}
