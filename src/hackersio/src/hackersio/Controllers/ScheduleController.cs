using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using hackersio.Models;
using Microsoft.AspNet.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace hackersio.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = null, Policy = null, Roles = "Admin")]
    public class ScheduleController : Controller
    {
        // GET: /<controller>/
        CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("your account name", "your account key"), true);
        public IActionResult Index()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference("myschedules");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<ScheduleCommand> query = new TableQuery<ScheduleCommand>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,"ROOM1"));

            return View(table.ExecuteQuery(query));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ScheduleCommand entity)
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

              // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference("myschedules");
            table.CreateIfNotExists();

            // Create a new entity.
            entity.PartitionKey = "ROOM1";
            entity.RowKey = Guid.NewGuid().ToString();
            entity.IsScheduled = true;
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(entity);

            // Execute the insert operation.
            table.Execute(insertOperation);

            return RedirectToAction("Index");
        }
    }
}
