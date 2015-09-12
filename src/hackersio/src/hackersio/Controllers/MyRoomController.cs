using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using hackersio.Models;
using Microsoft.AspNet.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace hackersio.Controllers
{
    [Authorize(ActiveAuthenticationSchemes =null,Policy =null,Roles ="Admin")]
    public class MyRoomController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var state = Request.Query["state"];

            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("your account name", "your account key"), true);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("myroomcommands");

            // Create a new customer entity.
            RoomCommand c;
            TableOperation insertOperation;
            if (state == "on1")
            {
                c = new RoomCommand("ROOM1", Guid.NewGuid().ToString());
                c.RelayId = "RELAY1";
                c.State = true;
                // Create the TableOperation object that inserts the customer entity.
                insertOperation = TableOperation.Insert(c);
                // Execute the insert operation.
                table.Execute(insertOperation);
            }

            if (state == "off1")
            {
                c = new RoomCommand("ROOM1", Guid.NewGuid().ToString());
                c.RelayId = "RELAY1";
                c.State = false;
                // Create the TableOperation object that inserts the customer entity.
                insertOperation = TableOperation.Insert(c);
                // Execute the insert operation.
                table.Execute(insertOperation);
            }

            if (state == "on2")
            {
                c = new RoomCommand("ROOM1", Guid.NewGuid().ToString());
                c.RelayId = "RELAY2";
                c.State = true;
                // Create the TableOperation object that inserts the customer entity.
                insertOperation = TableOperation.Insert(c);
                // Execute the insert operation.
                table.Execute(insertOperation);
            }

            if (state == "off2")
            {
                c = new RoomCommand("ROOM1", Guid.NewGuid().ToString());
                c.RelayId = "RELAY2";
                c.State = false;
                // Create the TableOperation object that inserts the customer entity.
                insertOperation = TableOperation.Insert(c);
                // Execute the insert operation.
                table.Execute(insertOperation);
            }

            return View();
        }

        
    }

    
}
