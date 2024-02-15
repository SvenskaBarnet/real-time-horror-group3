using Npgsql;
using System.Net;

namespace real_time_horror_group3;

public class Check()
{
    public static string Room(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        GameEvent.RandomTrigger(db);
        string? message = string.Empty;

        int roomId = PlayerPosition(db, request, response);

        NpgsqlCommand checkRoom = db.CreateCommand(@"
            SELECT COUNT(id)
            FROM room
            WHERE id = $1 AND has_danger = true
            ");
        checkRoom.Parameters.AddWithValue(roomId);
        using var reader = checkRoom.ExecuteReader();

        if (reader.Read())
        {
            if (reader.GetInt32(0) != 0)
            {
                message = "You found a dangerous object in this room, be careful!";
            }
            else
            {
                message = "This room is safe, no dangers to be found.";
            }
        }
        reader.Close();
        response.StatusCode = (int)HttpStatusCode.OK;
        return message;
    }
    public static string Windows(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        int roomId = PlayerPosition(db, request, response);
        bool hasDanger = Player.RoomHasDanger(db, request,response);
        string message = string.Empty;

        if (!hasDanger)
        {

            var windows = db.CreateCommand(@"
                        SELECT name, is_locked
                        FROM entry_point
                        WHERE room_id = $1 AND type = 'Window';
                        ");
            windows.Parameters.AddWithValue(roomId);
            using var reader2 = windows.ExecuteReader();

            while (reader2.Read())
            {
                switch (reader2.GetBoolean(1))
                {
                    case true:
                        message += $"Window {reader2.GetString(0)} is locked.\n";
                        break;
                    case false:
                        message += $"Window {reader2.GetString(0)} is unlocked.\n";
                        break;
                }
            }

            reader2.Close();
            GameEvent.RandomTrigger(db);
            response.StatusCode = (int)HttpStatusCode.OK;
            return message;
        }
        else
        {
            GameEvent.RandomTrigger(db);
            response.StatusCode = (int)HttpStatusCode.OK;
            message = "You forgot to clear the room of dangers and you are now dead.";
            return message;
        }

    }
    public static string Doors(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        int roomId = PlayerPosition(db, request, response);
        string message = string.Empty;
        bool hasDanger = Player.RoomHasDanger(db, request, response);

        if (!hasDanger)
        {
            var windows = db.CreateCommand(@"
                        SELECT name, is_locked
                        FROM entry_point
                        WHERE room_id = $1 AND type = 'Door';
                        ");
            windows.Parameters.AddWithValue(roomId);
            using var reader2 = windows.ExecuteReader();

            while (reader2.Read())
            {
                switch (reader2.GetBoolean(1))
                {
                    case true:
                        message += $"Door {reader2.GetString(0)} is locked.\n";
                        break;
                    case false:
                        message += $"Door {reader2.GetString(0)} is unlocked.\n";
                        break;
                }
            }
            GameEvent.RandomTrigger(db);
            response.StatusCode = (int)HttpStatusCode.OK;
            reader2.Close();
            return message;
        }
        else
        {
            GameEvent.RandomTrigger(db);
            response.StatusCode = (int)HttpStatusCode.OK;
            message = "You forgot to clear the room of dangers and you are now dead.";
            return message;
        }
    }

    public static string EntryPoints(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response, string playerName)
    {
        var playerPos = db.CreateCommand(@"
            SELECT p.location, r.name
            FROM public.player p
            JOIN public.room r ON r.id = p.location
            WHERE p.name = $1;
            ");

        playerPos.Parameters.AddWithValue(playerName ?? string.Empty);
        using var reader1 = playerPos.ExecuteReader();

        int roomId = 0;
        string roomName = string.Empty;
        if (reader1.Read())
        {
            roomId = reader1.GetInt32(0);
            roomName = reader1.GetString(1);
        }

        int doors = GetEntries(db, roomId, "Door");
        int windows = GetEntries(db, roomId, "Window");

        string message = $"You are in the {roomName}. \nThere is {doors} door(s) and {windows} window(s).";

        reader1.Close();
        return message;
    }

    private static int GetEntries(NpgsqlDataSource db, int roomId, string type)
    {
        var entryPoints = db.CreateCommand(@"
            SELECT COUNT(type) 
            FROM entry_point
            WHERE room_id = $1 AND type = $2;
            ");
        entryPoints.Parameters.AddWithValue(roomId);
        entryPoints.Parameters.AddWithValue(type);
        using var reader2 = entryPoints.ExecuteReader();

        int count = 0;
        while (reader2.Read())
        {
            count = reader2.GetInt32(0);
        }

        reader2.Close();
        return count;
    }
    public static int PlayerPosition(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        var playerPos = db.CreateCommand(@"
                        SELECT location
                        FROM public.player
                        WHERE name = $1
                        ");
        playerPos.Parameters.AddWithValue(Player.Verify(db, request, response));

        using var reader = playerPos.ExecuteReader();
        int roomId = 0;

        if (reader.Read())
        {
            roomId = reader.GetInt32(0);
        }

        reader.Close();
        return roomId;
    }
}