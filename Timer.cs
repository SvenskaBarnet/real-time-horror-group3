using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace real_time_horror_group3;

public class Timers
{
    int second = 59;
    int minute = 29;
    
    
    System.Timers.Timer gameSession = new(1000);
    public void GameSession()
    {
        // Attach the Tick method to the Elapsed event
        gameSession.Elapsed += Tick;
        // Enable the Timer
        gameSession.Enabled = true;

    }
    // gör detta till en curl kommando för att se hur mycket tid som är kvar
    public void Tick(Object timeCounter, ElapsedEventArgs elapsedTime)
    {
        
        Console.WriteLine($"Time left:{minute}:{second}"); 
        
        second--;
        if(minute == 0 && second == 0)
        {
            Console.WriteLine("Out of time: Game over");
            gameSession.Enabled = false;

        }
        if(second == 0)
        {
            minute--;
            second = 59;

        }
        
    }
    

}
