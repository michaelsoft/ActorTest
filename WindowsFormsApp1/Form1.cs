using Actor1.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        private static string url = "fabric:/MyActorApp/";
        private static string url2 = "fabric:/MyActorApp/Actor1ActorService";

        private List<string> messages = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var id = new ActorId(Guid.NewGuid());

            var actor = ActorProxy.Create<IActor1>(id, url);
            //Console.WriteLine($"ActorId: {id} - P: {id.GetPartitionKey()}");
            var msg = $"ActorId: {id} - P: {id.GetPartitionKey()}";
            messages.Add(msg);
            this.textBox1.Lines = messages.ToArray();

            actor.DoWorkAsync();

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using (var client = new FabricClient())
            {
                var serviceName = new Uri(url2);
                var partitions = await client.QueryManager.GetPartitionListAsync(serviceName);
                List<string> patitionInfo = new List<string>();

                foreach (var partition in partitions)
                {
                    //Debug.Assert(partition.PartitionInformation.Kind == ServicePartitionKind.Int64Range);
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
                            //Console.WriteLine(msg);
                            patitionInfo.Add(msg);
                        }

                        //activeActors.AddRange(page.Items.Where(x => x.IsActive));

                        continuationToken = page.ContinuationToken;
                    }
                    while (continuationToken != null);
                }

                this.textBox2.Lines = patitionInfo.ToArray();
            }
        }
    }
}
