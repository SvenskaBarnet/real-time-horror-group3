using Npgsql;
using System.Net;
using System.Text;
namespace real_time_horror_group3;

public class Player(NpgsqlDataSource db)
{
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

        string message = $"Player '{name}' has been created";
        response.StatusCode = (int)HttpStatusCode.Created;

        return message;
    }
}