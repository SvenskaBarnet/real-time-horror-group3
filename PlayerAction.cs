using Npgsql;
using System.Net;
using System.Reflection;

namespace real_time_horror_group3;

public class PlayerAction()
{
    public static string Lock(NpgsqlDataSource db, string type, HttpListenerRequest request, HttpListenerResponse response)
    {
        string message = string.Empty;
        StreamReader reader = new(request.InputStream, request.ContentEncoding);
        string lockName = reader.ReadToEnd();

        var selectChoise = db.CreateCommand(@$"
            SELECT COUNT(*) 
            FROM public.entry_point     
            WHERE name = $1
            ");
        selectChoise.Parameters.AddWithValue(lockName);
        selectChoise.ExecuteNonQuery();

        using var reader1 = selectChoise.ExecuteReader();

        int validChoise = 0;
        if (reader1.Read())
        {
            validChoise = reader1.GetInt32(0);
        }

        reader1.Close();




        // IF exists sen ska if-satsen  köras annars inte 
        bool hasDanger = Player.RoomHasDanger(db, request, response);
        if (!hasDanger)
        {
            if (validChoise < 0)
            {

                var cmd = db.CreateCommand(@$"

            UPDATE entry_point
            SET is_locked = true, ""time"" = null
            WHERE name = $1 AND room_id = $2 AND type = $3;");

                cmd.Parameters.AddWithValue(lockName);
                cmd.Parameters.AddWithValue(Check.PlayerPosition(db, request, response));
                cmd.Parameters.AddWithValue(type);
                cmd.ExecuteNonQuery();

                message = $"{type} {lockName} is now locked";
                response.StatusCode = (int)HttpStatusCode.OK;
                GameEvent.RandomTrigger(db);
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
        else
        {
            message = "Not a valid choice";
            return message;
        }
    }

    public static string RemoveDanger(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        int roomId = Check.PlayerPosition(db, request, response);

        NpgsqlCommand removeDanger = db.CreateCommand(@"
            UPDATE public.room
            SET has_danger = false
            WHERE id = $1;
            ");
        removeDanger.Parameters.AddWithValue(roomId);
        removeDanger.ExecuteNonQuery();

        string message = "You cleared the room of dangerous objects, it's safe now.";
        return message;
    }
}
