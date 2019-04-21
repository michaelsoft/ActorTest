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
using System.Fabric;
using System.Diagnostics;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;

namespace ConsoleApp1
{
    class Worker
    {
       private static string url = "fabric:/MyActorApp/";
        private static string url2 = "fabric:/MyActorApp/Actor1ActorService";

        public static async void DoWork()
        {

            for (var i = 1; i <= 10; i++)
            {
                var id = new ActorId(Guid.NewGuid());

                var actor = ActorProxy.Create<IActor1>(id, url);
                Console.WriteLine($"ActorId: {id} - P: {id.GetPartitionKey()}");

                actor.DoWorkAsync();

            }

            using (var client = new FabricClient())
            {
                var serviceName = new Uri(url2);
                var partitions = await client.QueryManager.GetPartitionListAsync(serviceName);

                foreach (var partition in partitions)
                {
                    Debug.Assert(partition.PartitionInformation.Kind == ServicePartitionKind.Int64Range);
                    var partitionInformation = (Int64RangePartitionInformation)partition.PartitionInformation;
                    //var proxy = ServiceProxy.Create<IActor1>(serviceName, new ServicePartitionKey(partitionInformation.LowKey));
                    var actorServiceProxy = ActorServiceProxy.Create(new Uri(url2), partitionInformation.LowKey);
 
                    ContinuationToken continuationToken = null;

                    do
                    {
                        PagedResult<ActorInformation> page = await actorServiceProxy.GetActorsAsync(continuationToken, CancellationToken.None);
                        foreach (var actor in page.Items)
                        {
                            var msg = $"P: {partitionInformation.LowKey} - {actor.ActorId} - {actor.IsActive}";
                            Console.WriteLine(msg);
                        }

                        //activeActors.AddRange(page.Items.Where(x => x.IsActive));

                        continuationToken = page.ContinuationToken;
                    }
                    while (continuationToken != null);
                }
            }


            //var partitionIds = new long[] { -3074457345618258603, 3074457345618258602, 9223372036854775807 };
            //var partitionIds = new string[] { "767931d4-e5b7-4e87-b8cf-185ae39ef9a5", "861ae5ff-648a-4e6d-928f-2729961cf6b3", "f9b8ebb8-a7e0-4c43-be84-28d106d58b7d" };
            //for (var i = 0; i < partitionIds.Length; i++)
            //{
            //    var pk = partitionIds[i];
                
            //    var actorServiceProxy = ActorServiceProxy.Create(new Uri(url2), pk.GetHashCode());

            //    ContinuationToken continuationToken = null;
            //    CancellationToken cancellationToken = new CancellationToken();
            //    //List<ActorInformation> activeActors = new List<ActorInformation>();

            //    do
            //    {
            //        PagedResult<ActorInformation> page = await actorServiceProxy.GetActorsAsync(continuationToken, CancellationToken.None);
            //        foreach (var actor in page.Items)
            //        {
            //            var msg = $"P: {pk} - {actor.ActorId} - {actor.IsActive}";
            //            Console.WriteLine(msg);
            //        }

            //        //activeActors.AddRange(page.Items.Where(x => x.IsActive));

            //        continuationToken = page.ContinuationToken;
            //    }
            //    while (continuationToken != null);


            //}

        }
    }
}
