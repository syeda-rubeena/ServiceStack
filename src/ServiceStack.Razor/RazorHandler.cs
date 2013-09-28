using System.Net;
using ServiceStack.Razor.Managers;
using ServiceStack.ServiceHost;
using ServiceStack.Support;
using ServiceStack.Support.WebHost;
using ServiceStack.Text;
using ServiceStack.Web;
using ServiceStack.Web.Handlers;

namespace ServiceStack.Razor
{
    public class RazorHandler : ServiceStackHandlerBase
    {
        public RazorFormat RazorFormat { get; set; }
        public RazorPage RazorPage { get; set; }
        public object Model { get; set; }

        public string PathInfo { get; set; }

        public RazorHandler(string pathInfo)
        {
            PathInfo = pathInfo;
        }

        public override void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            httpRes.ContentType = MimeTypes.Html;
            if (RazorFormat == null)
                RazorFormat = RazorFormat.Instance;

            var contentPage = RazorPage ?? RazorFormat.FindByPathInfo(PathInfo);
            if (contentPage == null)
            {
                httpRes.StatusCode = (int)HttpStatusCode.NotFound;
                httpRes.EndHttpHandlerRequest();
                return;
            }

            var model = Model;
            if (model == null)
                httpReq.Items.TryGetValue("Model", out model);
            if (model == null)
            {
                var modelType = RazorPage != null ? RazorPage.ModelType : null;
                model = modelType == null || modelType == typeof(DynamicRequestObject)
                    ? null
                    : DeserializeHttpRequest(modelType, httpReq, httpReq.ContentType);
            }

            RazorFormat.ProcessRazorPage(httpReq, contentPage, model, httpRes);
        }

        public override object CreateRequest(IHttpRequest request, string operationName)
        {
            return null;
        }

        public override object GetResponse(IHttpRequest httpReq, IHttpResponse httpRes, object request)
        {
            return null;
        }
    }
}