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
        string message = "As a player, I want to wake up in my house by the lake feeling tired and confused, so I can immerse myself in the storyline.";

        byte[] bytes = Encoding.UTF8.GetBytes(message);
        res.OutputStream.Write(bytes);
        res.Close();
    }

}

