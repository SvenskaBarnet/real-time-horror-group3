using System.Diagnostics.Metrics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Npgsql;
using real_time_horror_group3;


string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=notsohomealone";
await using var db = NpgsqlDataSource.Create(dbUri);
Database database = new(db);
await database.Create();

bool listen = true;
string? message = string.Empty;

Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("Interuping cancel event");
    e.Cancel = true;
    listen = false;
};

int port = 3000;
HttpListener listener = new();

listener.Prefixes.Add($"http://localhost:{port}/");
listener.Start();
Console.WriteLine($"Server listening on port: {port}");

listener.BeginGetContext(new AsyncCallback(Router), listener);
while (listen)
{

}

listener.Stop();
Console.WriteLine("Server stopped");


async void Router(IAsyncResult result)
{
    if (result.AsyncState is HttpListener listener)
    {
        HttpListenerContext context = listener.EndGetContext(result);
        HttpListenerResponse response = context.Response;
        HttpListenerRequest request = context.Request;

        response.ContentType = "text/plain";
        Player player = new(db);

        switch (request.Url?.AbsolutePath.ToLower())
        {

            case (string path) when path == "/new-player":
                if (request.HttpMethod is "POST")
                {
                    message = await player.Create(request, response);
                }
                break;
            case (string path) when path == $"/{await player.Verify(request, response)}/move":
                if (request.HttpMethod is "PATCH")
                {
                    message = await player.Move(request, response);
                }
                break;

            case "/new-session":
                Session session = new(db);

                if (request.HttpMethod is "GET")
                {
                    message = await session.Start(response);


                }
                break;
            default:
                message = "Not Found";
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
        }

        byte[] buffer = Encoding.UTF8.GetBytes(message);
        response.OutputStream.Write(buffer);
        response.OutputStream.Close();

        listener.BeginGetContext(new AsyncCallback(Router), listener);
    }

}