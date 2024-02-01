﻿using real_time_horror_group3;
using Npgsql;
using System.Net;

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
    while (listen) { };
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

    switch (request.HttpMethod)
    {
        case "GET":
            NotFound(response);
            break;
        default:
            NotFound(response);
            break;
           
    }
}

void NotFound(HttpListenerResponse response)
{
    response.StatusCode = (int)HttpStatusCode.NotFound;
    response.Close();
}
