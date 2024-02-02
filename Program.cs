using real_time_horror_group3;
using Npgsql;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Text;


string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=NotSoHomeAlone";
await using var db = NpgsqlDataSource.Create(dbUri);

//await Database.Create(db);

bool listen = true;


Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("Interupting cancel event");
    e.Cancel = true;
    listen = false;
};

int port = 3000;

HttpListener listener = new();
listener.Prefixes.Add($"http://localhost:{port}/");

try
{
    listener.Start();
    listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
    Console.WriteLine($"Server listening on port {port}");
    while (listen)
    {
    };
}
finally
{
    listener.Stop();
}

void HandleRequest(IAsyncResult result)
{
    if (result.AsyncState is HttpListener listener)
    {
        HttpListenerContext context = listener.EndGetContext(result);
        Router(context);
        listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);
    }
}

void Router(HttpListenerContext context)
{
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;

    switch (request.HttpMethod, request.Url?.AbsolutePath.ToLower())
    {
        case ("GET", string start) when start.StartsWith("/start"):
            
            switch (request.Url.AbsolutePath.ToLower())
            {
                case (string onlyStart) when onlyStart.EndsWith("/start"):
                    IntroStory intro = new IntroStory();
                    intro.CallStory(response);
                    break;
                case (string check) when check.EndsWith("/check"):

                    // skriva ut vilka alternativ som finns i rummet?

                    switch (request.Url.AbsolutePath.ToLower())
                    {
                        case (string door) when door.EndsWith("/door"):
                            Door(response);
                            break;

                        default:
                            NotFound(response);
                            break;
                    }
                    break;
                default:
                    break;
            }
            break;
        case ("GET", "/window"):
            Window(response);
            break;

        case ("GET", "/help"):
            Help(response);
            break;
        default:
            NotFound(response);
            break;
    }

}

void NotFound(HttpListenerResponse response)
{
    string message = "invaild option, try \"/help\"";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();

    response.StatusCode = (int)HttpStatusCode.NotFound;
    response.Close();
}

void Door(HttpListenerResponse response)
{
    string message = "Here is a door";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";
    response.StatusCode = (int)HttpStatusCode.OK;

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
}

void Window(HttpListenerResponse response)
{
    string message = "Here is a window";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";
    response.StatusCode = (int)HttpStatusCode.OK;

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
}

void Error(HttpListenerResponse response)
{
    string message = "Error code";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";
    response.StatusCode = (int)HttpStatusCode.OK;

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
}

void Help(HttpListenerResponse response)
{
    string message = "Available path \"/door\" and \"/window\"";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";
    response.StatusCode = (int)HttpStatusCode.OK;

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
}

void Check(HttpListenerResponse response)
{
    // hämta från databas vad som finns i rummet
    string message = "Available path \"/door\" and \"/window\"";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";
    response.StatusCode = (int)HttpStatusCode.OK;

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
}




