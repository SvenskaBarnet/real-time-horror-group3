using Npgsql;
using System.Net;
using System.Text;
namespace real_time_horror_group3;

public class Player()
{
    public static string Create(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string name = reader.ReadToEnd().ToLower();

         using var cmd = db.CreateCommand(@"
                    INSERT INTO public.player
                    (name, location)
                    VALUES($1, 1);
                    ");
        cmd.Parameters.AddWithValue(name);
         cmd.ExecuteNonQuery();

        string message = $"Player '{name}' has been created. Type /ready when you are ready. Game can only start when all players are ready.";
        response.StatusCode = (int)HttpStatusCode.Created;

        return message;
    }

    public static string Ready(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        string playerName =  Verify(db, request, response);

         using var cmd = db.CreateCommand(@"
        UPDATE public.player
        SET is_ready = true
        WHERE name = $1;
    ");
        cmd.Parameters.AddWithValue(playerName);

         cmd.ExecuteNonQuery();
        response.StatusCode = (int)HttpStatusCode.OK;

        string message = $"{playerName}, you are ready!";
        return message;
    }


    public static string Move(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)

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

        string playerName =  Verify(db, request, response);

         using var cmd = db.CreateCommand(@"
                        UPDATE public.player
                        SET location = $1
                        WHERE name = $2;
                        ");
        cmd.Parameters.AddWithValue(room);
        cmd.Parameters.AddWithValue(playerName);

         cmd.ExecuteNonQuery();

        GameEvent.RandomTrigger(db);

        response.StatusCode = (int)HttpStatusCode.OK;
        string message = $"{ Check.EntryPoints(db, request, response, playerName)}";

        return message;
    }
    public static string Verify(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        string? path = request.Url?.AbsolutePath;
        string? name = path?.Split('/')[1];

         using var cmd = db.CreateCommand(@"
            SELECT (name)
            FROM public.player
            WHERE name = $1
            ");
        cmd.Parameters.AddWithValue(name ?? string.Empty);
        var reader =  cmd.ExecuteReader();

        string username = string.Empty;
        if ( reader.Read())
        {
            username = reader.GetString(0);
        }

        return username;
    }

    public static bool CheckAllPlayersReady(NpgsqlDataSource db, HttpListenerResponse response)
    {
         using var cmd = db.CreateCommand(@"
        SELECT COUNT(*)
        FROM public.player
        ");

        var reader1 =  cmd.ExecuteReader();

        int totalPlayers = 0;
        if ( reader1.Read())
        {
            totalPlayers = reader1.GetInt32(0);
        }

         using var cmd1 = db.CreateCommand(@"
        SELECT COUNT(*) 
        FROM public.player 
        WHERE is_ready = true
        ");

        var reader2 =  cmd1.ExecuteReader();

        int readyPlayers = 0;
        if ( reader2.Read())
        {
            readyPlayers = reader2.GetInt32(0);
        }
        response.StatusCode = (int)HttpStatusCode.OK;
        if (totalPlayers == readyPlayers)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}