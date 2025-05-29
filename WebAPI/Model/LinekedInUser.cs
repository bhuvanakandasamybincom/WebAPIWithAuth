using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BoardCasterWebAPI.Model
{
    public class LinkedInUser
    {
        //[JsonPropertyName("id")]
        [JsonPropertyName("sub")]
        public string? Id { get; set; }

        //[JsonPropertyName("profilePicture")]
        //public ProfilePicture ProfilePicture { get; set; }

        [JsonPropertyName("vanityName")]
        public string? VanityName { get; set; }

        [JsonPropertyName("localizedFirstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("localizedLastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("localizedHeadline")]
        public string? Headline { get; set; }

        [JsonPropertyName("name")]
        public string? name { get; set; }
        //[JsonPropertyName("firstName")]
        //public LocalizedInformation LocalizedFirstName { get; set; }

        //[JsonPropertyName("lastName")]
        //public LocalizedInformation LocalizedLastName { get; set; }

        //[JsonPropertyName("headline")]
        //public LocalizedInformation LocalizedHeadline { get; set; }
    }
    public class LinkedInUserinfo
    {
        [JsonPropertyName("sub")]
        public string? id { get; set; }
        [JsonPropertyName("name")]
        public string? name { get; set; }
    }

    public class ShareRequest
    {
        /// <summary>
        /// The author of a share contains Person URN of the Member creating the share.
        /// </summary>
        /// <remarks>Needs to be formatted as a Person URn: Example "urn:li:person:{0}"</remarks>
        [JsonPropertyName("author")]
        public string? Author { get; set; }

        /// <summary>
        /// Defines the state of the share. For the purposes of creating a share, the lifecycleState will always be PUBLISHED.
        /// </summary>
        [JsonPropertyName("lifecycleState")]
        public string LifecycleState => "PUBLISHED";

        /// <summary>
        /// Provides additional options while defining the content of the share.
        /// </summary>
        [JsonPropertyName("specificContent")]
        public SpecificContent? SpecificContent { get; set; }

        [JsonPropertyName("visibility")]
        public Visibility? Visibility { get; set; }

    }
    //public class SpecificContent
    //{
    //    public object? ShareContent { get; set; }
    //}
    public class SpecificContent
    {
        [JsonPropertyName("com.linkedin.ugc.ShareContent")]
        public ShareContent? ShareContent { get; set; }
    }
    /// <summary>
    /// Represents the media assets attached to the share
    /// </summary>
    public enum ShareMediaCategoryEnum
    {
        /// <summary>
        /// The share does not contain any media, and will only consist of text.
        /// </summary>
        None,
        /// <summary>
        /// The contains a URL.
        /// </summary>
        Article,
        /// <summary>
        /// The share contains an image
        /// </summary>
        Image
    }
    public class ShareContent
    {
        private const string ShareMediaCategoryNone = "NONE";
        private const string ShareMediaCategoryArticle = "ARTICLE";
        private const string ShareMediaCategoryImage = "IMAGE";

        /// <summary>
        /// Provides the primary content for the share.
        /// </summary>
        [JsonPropertyName("shareCommentary")]
        public TextProperties? ShareCommentary { get; set; }

        /// <summary>
        /// Represents the media assets attached to the share.
        /// </summary>
        [JsonPropertyName("shareMediaCategory")]
        public string ShareMediaCategory => ShareMediaCategoryEnum switch
        {
            ShareMediaCategoryEnum.None => ShareMediaCategoryNone,
            ShareMediaCategoryEnum.Article => ShareMediaCategoryArticle,
            ShareMediaCategoryEnum.Image => ShareMediaCategoryImage,
            _ => ShareMediaCategoryNone
        };

        /// <summary>
        /// Represents the media assets attached to the share.
        /// </summary>
        [JsonIgnore]
        public ShareMediaCategoryEnum ShareMediaCategoryEnum { get; set; }

        /// <summary>
        /// If the shareMediaCategory is <see cref="ShareMediaCategoryEnum.Article"/> or <see cref="ShareMediaCategoryEnum.Image"/>, define those media assets here.
        /// </summary>
        [JsonPropertyName("media")]
        public Media[]? Media { get; set; }
    }
    public class Visibility
    {
        private const string VisibilityAnyone = "PUBLIC";
        private const string VisibilityConnectionsOnly = "CONNECTIONS";

        [JsonPropertyName("com.linkedin.ugc.MemberNetworkVisibility")]
        public string com_linkedin_ugc_MemberNetworkVisibility => VisibilityEnum switch
        {
            VisibilityEnum.Anyone => VisibilityAnyone,
            VisibilityEnum.ConnectionsOnly => VisibilityConnectionsOnly,
            _ => VisibilityAnyone
        };

        /// <summary>
        /// Defines any visibility restrictions for the share
        /// </summary>
        [JsonIgnore]
        public VisibilityEnum VisibilityEnum { get; set; }
    }
    public enum VisibilityEnum
    {
        /// <summary>
        /// The share will be viewable by anyone on LinkedIn
        /// </summary>
        Anyone,
        /// <summary>
        /// The share will be viewable by 1st-degree connections only
        /// </summary>
        ConnectionsOnly
    }
 
    public class TextProperties
    {
        public string? Text { get; set; }
    }

        public class ShareResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("serviceErrorCode")]
        public int? ServiceErrorCode { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        public bool IsSuccess => !string.IsNullOrEmpty(Id);
    }

    public class UploadRegistrationRequest
    {
        [JsonPropertyName("registerUploadRequest")]
        public RegisterUploadRequest? RegisterUploadRequest { get; set; }
    }

    public class UploadRegistrationResponse
    {
        [JsonPropertyName("value")]
        public Value? Value { get; set; }
    }
    public class MediaUploadHttpRequest
    {

        [JsonPropertyName("headers")]
        public Dictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// Use this URL to upload the media
        /// </summary>
        [JsonPropertyName("uploadUrl")]
        public string? UploadUrl { get; set; }
    }
    public class UploadMechanism
    {
        /// <summary>
        /// Details to upload media
        /// </summary>
        [JsonPropertyName("com.linkedin.digitalmedia.uploading.MediaUploadHttpRequest")]
        public MediaUploadHttpRequest? MediaUploadHttpRequest { get; set; }
    }
    public class Value
    {
        [JsonPropertyName("uploadMechanism")]
        public UploadMechanism? UploadMechanism { get; set; }

        [JsonPropertyName("mediaArtifact")]
        public string? MediaArtifact { get; set; }

        /// <summary>
        /// The identifier for the media. This is used when creating a share.
        /// </summary>
        /// <remarks>You will use this value for the corresponding Share</remarks>
        [JsonPropertyName("asset")]
        public string? Asset { get; set; }

        [JsonPropertyName("assetRealTimeTopic")]
        public string? AssetRealTimeTopic { get; set; }
    }
    public class RegisterUploadRequest
    {
        [JsonPropertyName("recipes")]
        public string[] Recipes => new[] { "urn:li:digitalmediaRecipe:feedshare-image" };

        /// <summary>
        /// The owner of the media. This can be the member who is sharing the media, or the organization that owns the media.
        /// </summary>
        /// <remarks>This should be in the Person Urn format. Example: urn:li:person:8675309</remarks>
        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("serviceRelationships")]
        public ServiceRelationships[]? ServiceRelationships { get; set; }
    }
    public class ServiceRelationships
    {
        [JsonPropertyName("relationshipType")]
        public string RelationshipType => "OWNER";

        [JsonPropertyName("identifier")]
        public string Identifier => "urn:li:userGeneratedContent";
    }
    /// <summary>
    /// The media to share on LinkedIn
    /// </summary>
    public class Media
    {
        /// <summary>
        /// Must be configured to "READY" to be shared.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status => "READY";

        /// <summary>
        /// Provide a short description for your image or article.
        /// </summary>
        [JsonPropertyName("description")]
        public TextProperties? Description { get; set; }

        /// <summary>
        /// ID of the uploaded image asset. If you are uploading an article, this field is not required
        /// </summary>
        /// <remarks>Must be in a URN format. Example: urn:li:digitalmediaAsset:D5622AQHqpGB5YNqcvg</remarks>
        [JsonPropertyName("media")]
        public string? MediaUrn { get; set; }

        /// <summary>
        /// Provide the URL of the article you would like to share here
        /// </summary>
        [JsonPropertyName("originalUrl")]
        public string? OriginalUrl { get; set; }

        /// <summary>
        /// Customize the title of your image or article
        /// </summary>
        [JsonPropertyName("title")]
        public TextProperties? Title { get; set; }
    }
}
