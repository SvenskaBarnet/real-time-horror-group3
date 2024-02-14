using Npgsql;
using System;
using System.Net;

namespace real_time_horror_group3;

public class Highscore()
{
    public static bool HandleGameOver(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {
        //string playerName = Player.Verify(db, request, response); 
        var cmd = db.CreateCommand(@"
      SELECT COUNT(*) 
      FROM public.player
      WHERE is_dead = true
        ");
        //cmd.Parameters.AddWithValue(playerName);  // Om alla är döda gameover oavsett namn, Player verify behövs inte

        var cmd2 = db.CreateCommand(@"
        SELECT COUNT(*) 
        FROM public.player;
        ");


        using var reader = cmd.ExecuteReader();
        int deadPlayer = 0;
        int totalPlayers = 0;


        if (reader.Read())
        {
            deadPlayer = reader.GetInt32(0);

        }

        using var reader2 = cmd2.ExecuteReader();
        if (reader2.Read())
        {
            totalPlayers = reader2.GetInt32(0);
        }

        reader.Close();
        reader2.Close();


        Console.WriteLine(deadPlayer + " " + totalPlayers);
        response.StatusCode = (int)HttpStatusCode.OK;
        if (deadPlayer == totalPlayers && totalPlayers != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void AddScore(NpgsqlDataSource db, HttpListenerRequest request, HttpListenerResponse response)
    {

        var selectName = db.CreateCommand(@"
        SELECT name 
        FROM public.player;
        ");

        using var reader = selectName.ExecuteReader();

        string playerNames = string.Empty;

        while (reader.Read())
        {
            playerNames += $"{reader.GetString(0)}, ";
        }

        reader.Close();

        string time = Session.FormattedTime(db);
        var highscore = db.CreateCommand(@"
        INSERT INTO public.highscore(player_names, ""time"")
        VALUES ($1, $2);
    ");
        highscore.Parameters.AddWithValue(playerNames);
        highscore.Parameters.AddWithValue(time);
        highscore.ExecuteNonQuery();
    }

}