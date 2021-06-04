using System.Net;
using System.Net.Http.Headers;

namespace Aijkl.CloudFlare.Internal
{
    public class APIResult<T>
    {        
        public T Result { set; get; }
        public EntityTagHeaderValue Etag { set; get; }
        public string ResponseBody { set; get; }
        public HttpResponseHeaders HttpResponseHeaders { set; get; }
        public HttpStatusCode HttpStatusCode { set; get; }
    }
}
