using Newtonsoft.Json;

namespace PhotoChallenge.Client.Helpers
{
    public static class ResponseHelper
    {
        public static async Task<string> GetErrorMessageFromResponseAsync(HttpResponseMessage? response) 
            => JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), new { Error = "" }).Error;
    }
}
