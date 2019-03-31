using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Actor1.Interfaces;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //for (var i=1; i<=10; i++)
            //{
            //    //ActorId id = ActorId.CreateRandom();
            //    var id = new ActorId(i);
            //    var url = "fabric:/MyActorApp";
            //    //var url = "http://localhost:19081/MyActorApp";
            //    var actor = ActorProxy.Create<IActor1>(id, url);

            //    actor.SetCountAsync(100).GetAwaiter().GetResult();
            //    var val = actor.GetCountAsync().GetAwaiter().GetResult();

            //    Console.WriteLine(i.ToString() + ": " + val);
            //}

            Worker.DoWork();

            Console.Read();
        }
    }
}
