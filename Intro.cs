﻿using System.Net;

namespace real_time_horror_group3;

public class Intro
{
    public static string Story(HttpListenerResponse respons)
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
        return message;
    }
   
}
