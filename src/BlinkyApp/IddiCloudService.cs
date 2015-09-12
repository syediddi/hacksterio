using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlinkyWebService
{
    public class IddiCloudService
    {
        CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("your account name", "your key"), true);
        // Create the table client.
        
        public IddiCloudService()
        {

            CreateIfNotExistsAsync();
            Insert();
        }

        public async void CreateIfNotExistsAsync()
        {
            try
            {
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                // Create the table if it doesn't exist.
                CloudTable table = tableClient.GetTableReference("myroomcommands");
                await table.CreateIfNotExistsAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }


        public async void Insert()
        {
            try
            {
                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference("myroomcommands");

                // Create a new customer entity.
                RoomCommand c = new RoomCommand("RELAY1", Guid.NewGuid().ToString());
                c.State = true;

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(c);

                // Execute the insert operation.
                await table.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
    }

    public class RoomCommand : TableEntity
    {
        public RoomCommand(string relayid, string guid)
        {
            this.PartitionKey = relayid;
            this.RowKey = guid;
        }

        public RoomCommand() { }
        public bool State { get; set; }
        public string RelayId { get; set; }
        
    }

    public class ScheduleCommand : RoomCommand
    {
        public ScheduleCommand(string key, string guid)
            : base(key, guid)
        {
        }

        public ScheduleCommand()
        {

        }
        public DateTime ScheduledTime { get; set; }
        public bool IsScheduled { get; set; }

    }
}
