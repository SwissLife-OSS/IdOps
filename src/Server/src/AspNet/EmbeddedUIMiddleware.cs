using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace IdOps.AspNet
{
    public class EmbeddedUIMiddleware
    {
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IFileProvider _fileProvider;
        private readonly RequestDelegate _next;

        public EmbeddedUIMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _fileProvider = new ManifestEmbeddedFileProvider(
                typeof(EmbeddedUIMiddleware).Assembly,
                "UI");

            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        public Task Invoke(HttpContext context)
        {
            var ct = _contentTypeProvider.TryGetContentType(
                context.Request.Path,
                out string contentType);

            var matchPath = context.Request.Path.Value;

            IFileInfo fileInfo = _fileProvider.GetFileInfo(matchPath);
            if (!fileInfo.Exists && !ct)
            {
                contentType = "text/html";
                fileInfo = _fileProvider.GetFileInfo("/index.html");
            }

            if (fileInfo.Exists)
            {
                SetHeaders(context, fileInfo);
                context.Response.ContentType = contentType;
                return context.Response.SendFileAsync(fileInfo);
            }

            return _next(context);
        }

        private static void SetHeaders(HttpContext context, IFileInfo fileInfo)
        {
            var length = fileInfo.Length;
            DateTimeOffset last = fileInfo.LastModified;

            DateTimeOffset lastModified = new DateTimeOffset(
                last.Year, last.Month, last.Day, last.Hour,
                last.Minute, last.Second, last.Offset)
                .ToUniversalTime();

            long etagHash = lastModified.ToFileTime() ^ length;
            var etag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue(
                '\"' + Convert.ToString(etagHash, 16) + '\"');

            ResponseHeaders headers = context.Response.GetTypedHeaders();
            headers.LastModified = fileInfo.LastModified;
            headers.ETag = etag;
        }
    }
}
