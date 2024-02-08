using Npgsql;
using System.Net;
using System.Text;
namespace real_time_horror_group3;

public class Player(NpgsqlDataSource db)
{
    Check check = new(db);
    public async Task<string> Create(HttpListenerRequest request, HttpListenerResponse response)
    {
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string name = reader.ReadToEnd();

        await using var cmd = db.CreateCommand(@"
                    INSERT INTO public.player
                    (name, location)
                    VALUES($1, 1);
                    ");
        cmd.Parameters.AddWithValue(name);
        await cmd.ExecuteNonQueryAsync();

        string message = $"Player '{name}' has been created.{await check.EntryPoints(request, response, name)}";
        response.StatusCode = (int)HttpStatusCode.Created;

        return message;
    }

    public async Task<string> Move(HttpListenerRequest request, HttpListenerResponse response)
        
    {
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string roomName = reader.ReadToEnd();
        int room = 0;
        switch (roomName.ToLower())
        {
            case "kitchen":
                room = 1;
                break;
            case "hallway":
                room = 2;
                break;
            case "living room":
                room = 3;
                break;
        }

        string playerName = await Verify(request, response);

        await using var cmd = db.CreateCommand(@"
                        UPDATE public.player
                        SET location = $1
                        WHERE name = $2;
                        ");
        cmd.Parameters.AddWithValue(room);
        cmd.Parameters.AddWithValue(playerName);

        await cmd.ExecuteNonQueryAsync();
        response.StatusCode = (int)HttpStatusCode.OK;
        string message = $"{await check.EntryPoints(request,response, playerName)}";

        return message;
    }
    public async Task<string> Verify(HttpListenerRequest request, HttpListenerResponse response)
    {
        string? path = request.Url?.AbsolutePath;
        string? name = path?.Split('/')[1];

        await using var cmd = db.CreateCommand(@"
            SELECT (name)
            FROM public.player
            WHERE name = $1
            ");
        cmd.Parameters.AddWithValue(name ?? string.Empty);
        var reader = await cmd.ExecuteReaderAsync();

        string username = string.Empty;
        if(await reader.ReadAsync())
        {
            username = reader.GetString(0);
        }
        return username;
    }

}