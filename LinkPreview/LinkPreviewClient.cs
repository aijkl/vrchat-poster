using Aijkl.LinkPreview.API.Clients;
using Aijkl.LinkPreview.Internal;
using System.Net.Http;

namespace Aijkl.LinkPreview.API
{
    public class LinkPreviewClient
    {
        private APIClient apiClient;
        private readonly string apikey;
        public LinkPreviewClient(string apikey ,HttpClient httpClient = null)
        {
            apiClient = new APIClient(httpClient ?? new HttpClient());
            this.apikey = apikey;
        }
        public MainClient Main => new MainClient(apiClient, apikey);
        public void Dispose()
        {
            apiClient?.Dispose();
            apiClient = null;
        }
    }
}
