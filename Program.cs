using real_time_horror_group3;
using Npgsql;
using System.Net;
using System.Text;


string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=NotSoHomeAlone";
await using var db = NpgsqlDataSource.Create(dbUri);

// await Database.Create(db);

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

async void Router(HttpListenerContext context)
{
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;
    Check check = new Check(db);

    switch (request.Url?.AbsolutePath.ToLower())
    {
        case (string path) when path.Equals("/start"):
            if (request.HttpMethod is "GET")
            {
                IntroStory intro = new IntroStory();
                intro.CallStory(response);
            }
            break;

        case (string path) when path.Equals("/check"):
            if (request.HttpMethod is "GET")
            {
                await check.Room(response);
            }
            break;

        case (string path) when path.Equals("/door"):

            if (request.HttpMethod is "GET")
            {
                await check.Door(response);
            }
            break;

        case (string path) when path.Equals("/window"):
            if (request.HttpMethod is "GET")
            {
                await check.Window(response);
            }
            break;

        case (string path) when path.Equals("/help"):
            if (request.HttpMethod is "GET")
            {
                Help(response);
            }
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

void Help(HttpListenerResponse response)
{
    string message = "Available path \"/door\" and \"/window\"";
    byte[] buffer = Encoding.UTF8.GetBytes(message);
    response.ContentType = "text/plain";
    response.StatusCode = (int)HttpStatusCode.OK;

    response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
}
