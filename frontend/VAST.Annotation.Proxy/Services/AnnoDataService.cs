using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VAST.Annotation.Proxy.Data;

namespace VAST.Annotation.Proxy.Services
{
    public class AnnoDataService
    {
        private static readonly HttpClient dataServiceClient;
        private static readonly HttpClient schemaServiceClient;

        private static readonly string collectionEndpoint = "api/collections";
        private static readonly string documentEndpoint = "api/collections/{0}/documents";
        private static readonly string annotationEndpoint = "api/collections/{0}/documents/{1}/annotations";
        
        private static readonly string schemaEndpoint = "clarin-ellogon-services/annotation_scheme.tcl/neutral/VAST_value/type/Generic";

#pragma warning disable S3963 // "static" fields should be initialized inline
        static AnnoDataService()
        {
            dataServiceClient = new HttpClient
            {
                BaseAddress = new Uri("https://annotation.vast-project.eu/")
            };
            dataServiceClient.DefaultRequestHeaders.Accept.Clear();
            dataServiceClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            schemaServiceClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.ellogon.org/")
            };
            schemaServiceClient.DefaultRequestHeaders.Accept.Clear();
            schemaServiceClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private string username;
        private string password;
        private AuthService authService;
        private string? accessToken = null;

        public AnnoDataService(string username, string password)
        {
            this.username = username;
            this.password = password;

            authService = new AuthService();
        }

        private async Task EnsureAccessToken(bool reauth = false)
        {
            if (reauth && accessToken == null)
            {
                throw new InvalidOperationException("There is no active access token, probably credentials are wrong!");

            }
            if (accessToken == null || reauth)
            {
                var authResponse = await authService.LoginAsync(username, password);
                if (authResponse != null && authResponse.success)
                {
                    accessToken = authResponse.access;
                    dataServiceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else
                {
                    throw new InvalidOperationException("Access token could not be retrieved with the provided credentials");
                }
            }
        }

        public async Task<CollectionListResponse?> GetCollections()
        {
            await EnsureAccessToken();
            var collectionResponse = await dataServiceClient.GetAsync(collectionEndpoint);
            if (collectionResponse.StatusCode == HttpStatusCode.Forbidden || collectionResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                await EnsureAccessToken(true);
                collectionResponse = await dataServiceClient.GetAsync(collectionEndpoint);
            }


            CollectionListResponse? responseData = await collectionResponse.Content.ReadFromJsonAsync<CollectionListResponse>();
            if (responseData != null && responseData.success)
            {
                return responseData;
            }
            else
            {
                throw new InvalidOperationException("Failed to get a response from the API.");
            }
        }

        public async Task<DocumentListResponse?> GetDocuments(int collectionId)
        {
            await EnsureAccessToken();
            var collectionResponse = await dataServiceClient.GetAsync(string.Format(documentEndpoint, collectionId));
            if (collectionResponse.StatusCode == HttpStatusCode.Forbidden || collectionResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                await EnsureAccessToken(true);
                collectionResponse = await dataServiceClient.GetAsync(string.Format(documentEndpoint, collectionId));
            }

            DocumentListResponse? responseData = await collectionResponse.Content.ReadFromJsonAsync<DocumentListResponse>();
            if (responseData != null && responseData.success)
            {
                return responseData;
            }
            else
            {
                throw new InvalidOperationException("Failed to get a response from the API.");
            }
        }

        public async Task<AnnotationListResponse?> GetAnnotations(int collectionId, int documentId)
        {
            await EnsureAccessToken();
            var collectionResponse = await dataServiceClient.GetAsync(string.Format(annotationEndpoint, collectionId, documentId));
            if (collectionResponse.StatusCode == HttpStatusCode.Forbidden || collectionResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                await EnsureAccessToken(true);
                collectionResponse = await dataServiceClient.GetAsync(string.Format(annotationEndpoint, collectionId, documentId));
            }

            AnnotationListResponse? responseData = await collectionResponse.Content.ReadFromJsonAsync<AnnotationListResponse>();
            if (responseData != null && responseData.success)
            {
                return responseData;
            }
            else
            {
                throw new InvalidOperationException("Failed to get a response from the API.");
            }
        }

        public async Task<SchemaResponse> GetSchema()
        {
            var schemaResponse = await schemaServiceClient.GetAsync(schemaEndpoint);

            if (schemaResponse.IsSuccessStatusCode)
            {
                return await schemaResponse.Content.ReadFromJsonAsync<SchemaResponse>();
            }
            else
            {
                throw new InvalidOperationException("Failed to get a response from the API.");
            }
        }
    }
}
