using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Common;

namespace MLock.Modules
{
    internal class WebServer
    {
        private CancellationTokenSource cts;
        private HttpListener listener;

        public async Task Initialize()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://+:4444/");

            listener.Start();

            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                ProcessRequest(context);
            }

            listener.Stop();
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var password = context.Request.QueryString.Get("password");
            if (password == null || password != Config.INSTANCE.WebServerPassword)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Close();
                return;
            }

            switch (context.Request.Url.AbsolutePath)
            {
                case "/lock":
                    Events.Lock();
                    break;
                case "/unlock":
                    Events.Unlock();
                    break;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;

            context.Response.Close();
        }

        private void
            StopServer(object sender,
                RoutedEventArgs e) // I am guessing server gets closed automatically when the program closes
        {
            cts?.Cancel();
        }
    }
}