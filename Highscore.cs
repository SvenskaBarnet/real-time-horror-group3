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
      SELECT COUNT(id) 
      FROM public.player
      WHERE is_dead = true

      UNION ALL

      SELECT COUNT(id) 
      FROM public.player;
        ");
        //cmd.Parameters.AddWithValue(playerName);  // Om alla är döda gameover oavsett namn, Player verify behövs inte



        using var reader = cmd.ExecuteReader();
        int deadPlayer = 0;
        int totalPlayers = 0;
        while (reader.Read())
        {
            deadPlayer = reader.GetInt32(0);
            totalPlayers = reader.GetInt32(1);
        }
        reader.Close();


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
        string playerName = Player.Verify(db, request, response);
        var time = Session.FormattedTime(db);
        var highscore = db.CreateCommand(@"
        INSERT INTO public.highscore(player_name, session_time)
        VALUES ($1, $2);
    ");
        highscore.Parameters.AddWithValue(playerName);
        highscore.Parameters.AddWithValue(time);
        highscore.ExecuteNonQuery();
    }

}