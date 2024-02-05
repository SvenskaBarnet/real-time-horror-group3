using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace real_time_horror_group3;

public class Timers
{
    DateTime? gameStartTime = null;
    TimeSpan gameTimeLimit = TimeSpan.FromSeconds(30);

    public bool GameStarted()
    {
        return gameStartTime.HasValue;
    }

    public void StartGame()
    {
        if (gameStartTime == null)
        {
            gameStartTime = DateTime.Now;

        }
        else
        {
            throw new InvalidOperationException("The game has already started.");
        }
    }

    public void CheckTimeLeft(HttpListenerResponse response)
    {
        if (gameStartTime.HasValue)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan elapsedTime = currentTime - gameStartTime.Value;
            TimeSpan timeLeft = gameTimeLimit - elapsedTime;

            if (timeLeft <= TimeSpan.Zero)
            {
                string message = "Game over. Time limit exceeded.";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.OK;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            else
            {
                string message = $"Time left: {timeLeft.TotalMinutes:F2} minutes";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                response.ContentType = "text/plain";
                response.StatusCode = (int)HttpStatusCode.OK;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
        else
        {
            string message = "Game has not started yet.";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentType = "text/plain";
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}
