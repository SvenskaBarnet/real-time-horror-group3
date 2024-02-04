using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace real_time_horror_group3;

    public class IntroStory
{
    public void CallStory(HttpListenerResponse res)
    {
        string message = @"
You wake up in your house by the lake, tired and confused about a sound you hear outside. 
You are sure that you are home alone and when you go to the window you see two dark figures,
could this be the burglars that have been all over the news lately? 
You estimate you have 5 minutes until they reach your house.
while talking to the police they said they will arrive in 30 minutes to save you.
Hurry up and secure the home by locking all windows and doors.
But be aware of your surroundings so you don't kill yourself.
Check every room in the house carefully.
Events could happen anytime.
You need to continue to check the status in every room during your remaining time period.";

        foreach (char character in message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(character.ToString());
            res.OutputStream.Write(bytes);
            Thread.Sleep(80);
        }
        res.Close();

    }

}

