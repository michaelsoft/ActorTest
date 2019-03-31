using Actor1.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Worker
    {
       private static string url = "fabric:/MyActorApp";

        public static async void DoWork()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            for (var i = 1; i <= 10; i++)
            {
                var id = new ActorId(i);
                
                var actor = ActorProxy.Create<IActor1>(id, url);
                
                await actor.DoWorkAsync();

            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.Seconds);
        }
    }
}
