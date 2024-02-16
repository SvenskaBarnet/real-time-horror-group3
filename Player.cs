using Npgsql;
using System.Net;
namespace real_time_horror_group3;

public class Player()
{
    public static string Create(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        


        string checkIfNameExists = reader.ReadToEnd();
        var selectChoice = db.CreateCommand(@$"
            SELECT COUNT(*)
            FROM public.player
            WHERE name = $1
            ");
        selectChoice.Parameters.AddWithValue(checkIfNameExists);

        using var checkName = selectChoice.ExecuteReader();

        int validChoice = 0;
        if (checkName.Read())
        {
            validChoice = checkName.GetInt32(0);
        }
        reader.Close();

        if (validChoice > 0)
        {
            return "Player with this name already exists.";
        }

        string name = checkIfNameExists.ToLower();

        var cmd = db.CreateCommand(@"
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
        string playerName = Verify(db, request);

        var cmd = db.CreateCommand(@"
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
        string message = string.Empty;
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string roomName = reader.ReadToEnd();
        int roomId = 0;

        switch (roomName.ToLower())
        {
            case "kitchen":
                roomId = 1;
                break;
            case "hallway":
                roomId = 2;
                break;
            case "living room":
                roomId = 3;
                break;
        }

        if (roomId != 0)
        {
            string playerName = Verify(db, request);

            bool hasDanger = Player.RoomHasDanger(db, request, response);

            var cmd = db.CreateCommand(@"
                        UPDATE public.player
                        SET location = $1
                        WHERE name = $2;
                        ");
            cmd.Parameters.AddWithValue(roomId);
            cmd.Parameters.AddWithValue(playerName);

            cmd.ExecuteNonQuery();

            GameEvent.RandomTrigger(db);

            if (hasDanger)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                message = "You forgot to check for dangers, you are now dead!";
                return message;
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                message = $"{Check.EntryPoints(db, request, response, playerName)}";
                return message;
            }
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            message = "Invalid room name";
            return message;
        }
    }

    public static bool RoomHasDanger(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        var cmd = db.CreateCommand(@"
        SELECT has_danger
        FROM public.room
        WHERE id = $1;
    ");
        cmd.Parameters.AddWithValue(Check.PlayerPosition(db, request, response));

        using var reader = cmd.ExecuteReader();

        bool hasDanger = false;
        if (reader.Read())
        {
            hasDanger = reader.GetBoolean(0);
        }
        reader.Close();

        if (hasDanger)
        {
            var killPlayer = db.CreateCommand(@"
                UPDATE public.player
                SET is_dead = true
                WHERE name = $1
                ");
            killPlayer.Parameters.AddWithValue(Player.Verify(db, request));
            killPlayer.ExecuteNonQuery();
        }
        return hasDanger;
    }

    public static string Verify(NpgsqlDataSource db, HttpListenerRequest request)
    {
        string? path = request.Url?.AbsolutePath;
        string? name = path?.Split('/')[1] ?? string.Empty;
        string username = string.Empty;

        var cmd = db.CreateCommand(@"
            SELECT (name)
            FROM public.player
            WHERE name = $1
            ");
        cmd.Parameters.AddWithValue(name ?? string.Empty);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            username = reader.GetString(0);
        }

        reader.Close();
        return username;
    }

    public static bool CheckAllPlayersReady(NpgsqlDataSource db, HttpListenerResponse response)
    {
        using var cmd = db.CreateCommand(@"
        SELECT COUNT(*)
        FROM public.player
        ");

        using var reader1 = cmd.ExecuteReader();

        int totalPlayers = 0;
        if (reader1.Read())
        {
            totalPlayers = reader1.GetInt32(0);
        }

        reader1.Close();

        var cmd1 = db.CreateCommand(@"
        SELECT COUNT(*) 
        FROM public.player 
        WHERE is_ready = true
        ");

        using var reader2 = cmd1.ExecuteReader();

        int readyPlayers = 0;
        if (reader2.Read())
        {
            readyPlayers = reader2.GetInt32(0);
        }
        response.StatusCode = (int)HttpStatusCode.OK;

        reader2.Close();
        if (totalPlayers == readyPlayers)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Death(NpgsqlDataSource db, string playerName)
    {
        var cmd = db.CreateCommand(@"
        SELECT name, is_dead
        FROM public.player
        WHERE name = $1 AND is_dead = true;
        ");
        cmd.Parameters.AddWithValue(playerName);

        using var reader = cmd.ExecuteReader();
        bool playerDeath = false;

        if (reader.Read())
        {
            playerDeath = reader.GetBoolean(1);
        }
        reader.Close();
        return playerDeath;
    }
}