﻿using System.Diagnostics.Metrics;
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
bool sessionStarted = false;
string? message = string.Empty;

Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("Interupting cancel event");
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
        Check check = new(db);
        PlayerAction action = new(db);

        switch (request.Url?.AbsolutePath.ToLower())
        {
            case (string path) when path == "/new-player":
                if (request.HttpMethod is "POST")
                {
                    message = await player.Create(request, response);
                }
                break;
            case (string path) when path == $"/{await player.Verify(request, response)}/start": // även lägga till att /ready måste va true.
                if (request.HttpMethod is "GET")
                {
                    Intro intro = new Intro();
                    message = await intro.Story(response);
                   
                }
                break;
            case (string path) when path == $"/{await player.Verify(request, response)}/move":
                if (sessionStarted)
                {
                    if (request.HttpMethod is "PATCH")
                    {
                        message = await player.Move(request, response);
                    }
                }
                else
                {
                    message = "You need to start session to play";
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                break;
            case (string path) when path == $"/{await player.Verify(request, response)}/windows":
                if (sessionStarted)
                {
                    if (request.HttpMethod is "GET")
                    {
                        message = await check.Windows(request, response, player);
                    }
                    else if (request.HttpMethod is "PATCH")
                    {
                        message = await action.Lock("Window", check, player, request, response);
                    }
                }
                else
                {
                    message = "You need to start session to play";
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                break;
            case (string path) when path == $"/{await player.Verify(request, response)}/doors":
                if (sessionStarted)
                {
                    if (request.HttpMethod is "GET")
                    {
                        message = await check.Doors(request, response, player);
                    }
                    else if (request.HttpMethod is "PATCH")
                    {
                        message = await action.Lock("Door", check, player, request, response);
                    }
                }
                else
                {
                    message = "You need to start session to play";
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                break;

            case (string path) when path == $"/{await player.Verify(request, response)}/new-session":
                Session session = new(db);

                if (request.HttpMethod is "GET")
                {
                    message = await session.Start(response);
                    if (message.Contains("started"))
                    {
                        sessionStarted = true;
                    }
                }
                break;
            default:
                message = "Not Found";
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
        }

        message = $"\n\n{message}\n\n";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        response.OutputStream.Write(buffer);
        response.OutputStream.Close();
        message = string.Empty;

        listener.BeginGetContext(new AsyncCallback(Router), listener);
    }
}