using real_time_horror_group3;
using Npgsql;
using System.Net;
using System.Text;


string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=NotSoHomeAlone";
await using var db = NpgsqlDataSource.Create(dbUri);

await Database.Create(db);

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

        case (string path) when path.Equals("/kitchen/check"):
            if (request.HttpMethod is "GET")
            {
                await check.Room(response, 1);
            }
            break;

        case (string path) when path.Equals("/livingroom/check"):
            if (request.HttpMethod is "GET")
            {
                await check.Room(response, 2);
            }
            break;

        case (string path) when path.Equals("/kitchen/door"):

            if (request.HttpMethod is "GET")
            {
                await check.Door(response, 1);
            }
            else if (request.HttpMethod is "PATCH")
            {
                Locking locking = new(db);
                await locking.Lock("Door", 1, request, response);
            }
            break;

        case (string path) when path.Equals("/livingroom/door/"):

            if (request.HttpMethod is "GET")
            {
                await check.Door(response, 2);
            }
            else if (request.HttpMethod is "PATCH")
            {
                Locking locking = new(db);
                await locking.Lock("Door", 2, request, response);
            }

            break;

        case (string path) when path.Equals("/kitchen/window/"):
            if (request.HttpMethod is "GET")
            {
                await check.Window(response, 1);
            }
            else if (request.HttpMethod is "PATCH")
            {
                Locking locking = new(db);
                await locking.Lock("Window", 1, request, response);
            }
            break;
          

        case (string path) when path.Equals("/livingroom/window/"):
            if (request.HttpMethod is "GET")
            {
                await check.Window(response, 2);
            }
            else if (request.HttpMethod is "PATCH")
            {
                Locking locking = new(db);
                await locking.Lock("Window", 2, request, response);
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



