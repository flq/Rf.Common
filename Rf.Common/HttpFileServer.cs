using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Rf.Common
{
    /// <summary>
    /// Typical position of hosts file : C:\Windows\System32\drivers\etc
    /// Command to set 
    /// A torrent client like utorrent is not willing
    /// 
    /// </summary>
    public class HttpFileServer : IDisposable
    {
        private readonly string rootPath;
        private const int BufferSize = 1024 * 512; //512KB
        private readonly HttpListener http;

        public HttpFileServer(string rootPath, int port)
        {
            this.rootPath = rootPath;
            http = new HttpListener();
            http.Prefixes.Add("http://localhost:" + port + "/");
            http.Start();
            http.BeginGetContext(requestWait, null);
        }

        public void Dispose()
        {
            http.Stop();
        }

        private void requestWait(IAsyncResult ar)
        {
            if (!http.IsListening)
                return;
            var c = http.EndGetContext(ar);
            http.BeginGetContext(requestWait, null);

            var url = tuneUrl(c.Request.RawUrl);

            var fullPath = string.IsNullOrEmpty(url) ? rootPath : Path.Combine(rootPath, url);

            if (Directory.Exists(fullPath))
                returnDirContents(c, fullPath);
            else if (File.Exists(fullPath))
                returnFile(c, fullPath);
            else
                return404(c);
        }

        private void returnDirContents(HttpListenerContext context, string dirPath)
        {

            context.Response.ContentType = "text/html";
            context.Response.ContentEncoding = Encoding.UTF8;
            using (var sw = new StreamWriter(context.Response.OutputStream))
            {
                sw.WriteLine("<html>");
                sw.WriteLine("<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head>");
                sw.WriteLine("<body><ul>");

                var dirs = Directory.GetDirectories(dirPath);
                foreach (var d in dirs)
                {
                    var link = d.Replace(rootPath, "").Replace('\\', '/');
                    sw.WriteLine("<li>&lt;DIR&gt; <a href=\"" + link + "\">" + Path.GetFileName(d) + "</a></li>");
                }

                var files = Directory.GetFiles(dirPath);
                foreach (var f in files)
                {
                    var link = f.Replace(rootPath, "").Replace('\\', '/');
                    sw.WriteLine("<li><a href=\"" + link + "\">" + Path.GetFileName(f) + "</a></li>");
                }

                sw.WriteLine("</ul></body></html>");
            }
            context.Response.OutputStream.Close();
        }

        private static void returnFile(HttpListenerContext context, string filePath)
        {
            context.Response.ContentType = getcontentType(Path.GetExtension(filePath));
            var buffer = new byte[BufferSize];
            using (var fs = File.OpenRead(filePath))
            {
                context.Response.ContentLength64 = fs.Length;
                int read;
                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, read);
            }

            context.Response.OutputStream.Close();
        }

        private static void return404(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
        }

        private static string tuneUrl(string url)
        {
            url = url.Replace('/', '\\');
            url = HttpUtility.UrlDecode(url, Encoding.UTF8);
            url = url.Substring(1);
            return url;
        }

        private static string getcontentType(string extension)
        {
            switch (extension)
            {
                case ".avi": return "video/x-msvideo";
                case ".css": return "text/css";
                case ".doc": return "application/msword";
                case ".gif": return "image/gif";
                case ".htm":
                case ".html": return "text/html";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".js": return "application/x-javascript";
                case ".mp3": return "audio/mpeg";
                case ".png": return "image/png";
                case ".pdf": return "application/pdf";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".zip": return "application/zip";
                case ".txt": return "text/plain";
                default: return "application/octet-stream";
            }
        }
    }
}