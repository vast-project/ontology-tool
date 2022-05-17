using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VAST.Annotation.Proxy.Data;

namespace VAST.Annotation.Proxy.Services
{
    public class AuthService
    {
        private static readonly HttpClient authServiceClient;

        private static readonly string loginEndpoint = "api/auth/login";

#pragma warning disable S3963 // "static" fields should be initialized inline
        static AuthService()
        {
            authServiceClient = new HttpClient
            {
                BaseAddress = new Uri("https://annotation.vast-project.eu/")
            };
            authServiceClient.DefaultRequestHeaders.Accept.Clear();
            authServiceClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<AuthenticationResponse?> LoginAsync(string email, string password)
        {
            var request = new AuthRequest {email = email, password = password, remember_me = false};

            //.NET BUG: Sets the content-length to 0 (https://github.com/aspnet/AspNetWebStack/issues/252)
            //var loginResponse = await authServiceClient.PostAsJsonAsync(loginEndpoint, request);

            //Workaround:
            var objectContent = JsonSerializer.Serialize<AuthRequest>(request);
            var requestContent = new StringContent(objectContent, Encoding.UTF8, "application/json");

            var loginResponse = await authServiceClient.PostAsync(loginEndpoint, requestContent);

            AuthenticationResponse? responseData = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();

            return responseData;
        }
    }
}
