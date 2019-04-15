using Actor1.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace ConsoleApp1
{
    class Worker
    {
       private static string url = "fabric:/MyActorApp/";
        private static string url2 = "fabric:/MyActorApp/Actor1ActorService";

        public static async void DoWork()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            for (var i = 1; i <= 10; i++)
            {
                var id = new ActorId(i);
                
                var actor = ActorProxy.Create<IActor1>(id, url);
                Console.WriteLine("P: " + id.GetPartitionKey().ToString());

                await actor.DoWorkAsync();

            }

            for (var i = 1; i <= 3; i++)
            {
                var actorServiceProxy = ActorServiceProxy.Create(new Uri(url2), i);

                ContinuationToken continuationToken = null;
                CancellationToken cancellationToken = new CancellationToken();
                //List<ActorInformation> activeActors = new List<ActorInformation>();

                do
                {
                    PagedResult<ActorInformation> page = await actorServiceProxy.GetActorsAsync(continuationToken, cancellationToken);
                    foreach (var actor in page.Items)
                    {
                        var msg = $"P: {i} - {actor.ActorId} - {actor.IsActive}";
                        Console.WriteLine(msg);
                    }

                    //activeActors.AddRange(page.Items.Where(x => x.IsActive));

                    continuationToken = page.ContinuationToken;
                }
                while (continuationToken != null);


            }
                stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.Seconds);
        }
    }
}
