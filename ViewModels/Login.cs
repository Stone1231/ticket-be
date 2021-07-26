using Newtonsoft.Json;

namespace Backend.ViewModels
{
    public class Login
    {
        [JsonProperty(PropertyName = "username")] 
        public string Username { get; set; }

        [JsonProperty(PropertyName = "pwd")] 
        public string Password { get; set; }

    }
}