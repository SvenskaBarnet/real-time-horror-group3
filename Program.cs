using System.Diagnostics.Metrics;
using System.Net;
using Npgsql;
using real_time_horror_group3;


string dbUri = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=notsohomealone";
await using var db = NpgsqlDataSource.Create(dbUri);
Database database = new(db);
await database.Create();

bool listen = true;
string? message = string.Empty;

Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) // när vi trycker på crtl+C så avbryt eventet.
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

listener.BeginGetContext(new AsyncCallback(Router), listener); // låter vår Startar en egen "bubbla" som kan hantera vår request(router), state = listener "state" en låda som kan skicka runt och titta i den och ha tillgång till den
while (listen)
{

}

listener.Stop();
Console.WriteLine("Server stopped");


async void Router(IAsyncResult result)
{

}