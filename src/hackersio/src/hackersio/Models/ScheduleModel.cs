using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hackersio.Models
{
    public class ScheduleViewModel
    {
    }

    public class RoomCommand : TableEntity
    {
        public RoomCommand(string key, string guid)
        {
            this.PartitionKey = key;
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
