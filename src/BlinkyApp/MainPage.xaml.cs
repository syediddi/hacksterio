// Copyright (c) Microsoft. All rights reserved.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using Windows.ApplicationModel.AppService;
using Windows.Devices.Gpio;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BlinkyWebService
{
    public sealed partial class MainPage : Page
    {
        AppServiceConnection appServiceConnection;
        FtdiSampleViewModel ftdi;
        DispatcherTimer schedule = new DispatcherTimer();
        DispatcherTimer queue = new DispatcherTimer();
        public MainPage()
        {
            InitializeComponent();
            //InitGPIO();
            InitAppSvc();
            ftdi = new FtdiSampleViewModel();


            //new IddiCloudService();



            queue.Tick += ProcessCloudQueue;
            queue.Interval = TimeSpan.FromMilliseconds(1000);
            queue.Start();



            schedule.Tick += ProcessCloudSchedules;
            schedule.Interval = TimeSpan.FromMilliseconds(30000);
            schedule.Start();
        }

        private readonly object locker = new object();
        CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("iddihackersio", "K6BnjqGL/n+C1mnbytxXq9iHgFfePph38NhEo6BndIqAXLlAFOIezmtNFTXwHO40/lulLd3YcLoXW3iEKoKM7g=="), true);

        private async void ProcessCloudSchedules(object sender, object e)
        {

            schedule.Tick -= ProcessCloudSchedules;
            // Create the table client.
            CloudTableClient scheduleTableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable scheduleTable = scheduleTableClient.GetTableReference("myschedules");

            // Create the table query.
            TableQuery<ScheduleCommand> scheduleQuery = new TableQuery<ScheduleCommand>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "ROOM1"));

            // Print the fields for each customer.
            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;
            do
            {

                // Retrieve a segment (up to 1,000 entities).
                TableQuerySegment<ScheduleCommand> tableQueryResult =
                    await scheduleTable.ExecuteQuerySegmentedAsync(scheduleQuery, continuationToken);

                // Assign the new continuation token to tell the service where to
                // continue on the next iteration (or null if it has reached the end).
                continuationToken = tableQueryResult.ContinuationToken;

                foreach (ScheduleCommand entity in tableQueryResult)
                {
                    //Added 3 hours as offset between pi and est
                    if (entity.ScheduledTime <= DateTime.Now.AddHours(3))
                    {
                        TableOperation deleteOperation = TableOperation.Delete(entity);
                        if (entity.RelayId == "RELAY1" && entity.State == true)
                        {
                            FAN.Fill = greenBrush;
                            StateText.Text = "Fan On";
                            TurnRelay1On();
                        }
                        else if (entity.RelayId == "RELAY1" && entity.State == false)
                        {
                            FAN.Fill = grayBrush;
                            StateText.Text = "Fan Off";
                            TurnRelay1Off();
                        }
                        else if (entity.RelayId == "RELAY2" && entity.State == true)
                        {
                            LIGHT.Fill = greenBrush;
                            StateTextlight.Text = "Light On";
                            TurnRelay2On();
                        }
                        else if (entity.RelayId == "RELAY2" && entity.State == false)
                        {
                            LIGHT.Fill = grayBrush;
                            StateTextlight.Text = "Light Off";
                            TurnRelay2Off();
                        }
                        else
                        {
                            FAN.Fill = redBrush;
                            LIGHT.Fill = redBrush;
                            StateText.Text = "Invalid Operation";
                            StateTextlight.Text = "Invalid Operation";
                        }
                        // Execute the operation.
                        await scheduleTable.ExecuteAsync(deleteOperation);
                    }
                }
            } while (continuationToken != null);


            schedule.Tick += ProcessCloudSchedules;
            schedule.Interval = TimeSpan.FromMilliseconds(30000);
            schedule.Start();
        }

        private async void ProcessCloudQueue(object sender, object e)
        {

            queue.Tick -= ProcessCloudQueue;

            // Create the table client.

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(); ;

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("myroomcommands");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<RoomCommand> query = new TableQuery<RoomCommand>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "ROOM1"));


            // Print the fields for each customer.
            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;
            do
            {

                // Retrieve a segment (up to 1,000 entities).
                TableQuerySegment<RoomCommand> tableQueryResult =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                // Assign the new continuation token to tell the service where to
                // continue on the next iteration (or null if it has reached the end).
                continuationToken = tableQueryResult.ContinuationToken;

                foreach (RoomCommand cmd in tableQueryResult)
                {
                    TableOperation deleteOperation = TableOperation.Delete(cmd);
                    if (cmd.RelayId == "RELAY1" && cmd.State == true)
                    {
                        FAN.Fill = greenBrush;
                        StateText.Text = "Fan On";
                        TurnRelay1On();
                    }
                    else if (cmd.RelayId == "RELAY1" && cmd.State == false)
                    {
                        FAN.Fill = grayBrush;
                        StateText.Text = "Fan Off";
                        TurnRelay1Off();


                    }
                    else if (cmd.RelayId == "RELAY2" && cmd.State == true)
                    {
                        LIGHT.Fill = greenBrush;
                        StateTextlight.Text = "Light On";
                        TurnRelay2On();
                    }
                    else if (cmd.RelayId == "RELAY2" && cmd.State == false)
                    {
                        LIGHT.Fill = grayBrush;
                        StateTextlight.Text = "Light Off";
                        TurnRelay2Off();


                    }
                    else
                    {
                        FAN.Fill = grayBrush;
                        LIGHT.Fill = grayBrush;
                        StateText.Text = StateTextlight.Text = "Invalid Operation";
                    }
                    // Execute the operation.
                    await table.ExecuteAsync(deleteOperation);
                }
            } while (continuationToken != null);

            queue.Tick += ProcessCloudQueue;
            queue.Interval = TimeSpan.FromMilliseconds(1000);
            queue.Start();
        }
        private void InitGPIO()
        {
            //var gpio = GpioController.GetDefault();

            //// Show an error if there is no GPIO controller
            //if (gpio == null)
            //{
            //    pin = null;
            //    GpioStatus.Text = "There is no GPIO controller on this device.";
            //    return;
            //}

            //pin = gpio.OpenPin(LED_PIN);
            //pin.Write(GpioPinValue.High);
            //pin.SetDriveMode(GpioPinDriveMode.Output);

            //GpioStatus.Text = "GPIO pin initialized correctly.";
        }

        private async void InitAppSvc()
        {
            // Initialize the AppServiceConnection
            appServiceConnection = new AppServiceConnection();
            appServiceConnection.PackageFamilyName = "WebServer_hz258y3tkez3a";
            appServiceConnection.AppServiceName = "App2AppComService";

            // Send a initialize request 
            var res = await appServiceConnection.OpenAsync();
            if (res == AppServiceConnectionStatus.Success)
            {
                var message = new ValueSet();
                message.Add("Command", "Initialize");
                var response = await appServiceConnection.SendMessageAsync(message);
                if (response.Status != AppServiceResponseStatus.Success)
                {
                    throw new Exception("Failed to send message");
                }
                appServiceConnection.RequestReceived += OnMessageReceived;
            }
        }

        private async void OnMessageReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string newState = message["State"] as string;
            switch (newState)
            {
                case "On1":
                    {
                        await Dispatcher.RunAsync(
                              CoreDispatcherPriority.High,
                             () =>
                             {
                                 TurnRelay1On();
                                 FAN.Fill = greenBrush;
                                 StateText.Text = "Fan On";
                             });
                        break;
                    }
                case "Off1":
                    {
                        await Dispatcher.RunAsync(
                        CoreDispatcherPriority.High,
                        () =>
                        {
                            TurnRelay1Off();
                            StateText.Text = "Fan Off";
                            FAN.Fill = grayBrush;
                        });
                        break;
                    }
                case "On2":
                    {
                        await Dispatcher.RunAsync(
                              CoreDispatcherPriority.High,
                             () =>
                             {
                                 TurnRelay2On();
                                 LIGHT.Fill = greenBrush;
                                 StateTextlight.Text = "Light On";


                             });
                        break;
                    }
                case "Off2":
                    {
                        await Dispatcher.RunAsync(
                        CoreDispatcherPriority.High,
                        () =>
                        {
                            TurnRelay2Off();
                            StateTextlight.Text = "Light Off";
                            LIGHT.Fill = grayBrush;
                        });
                        break;
                    }
                case "Unspecified":
                default:
                    {
                        // Do nothing 
                        break;
                    }
            }
        }

        private void FlipLED()
        {
            //if (LEDStatus == 0)
            //{
            //    LEDStatus = 1;
            //    if (pin != null)
            //    {
            //        // to turn on the LED, we need to push the pin 'low'
            //        pin.Write(GpioPinValue.Low);
            //    }
            //    LED.Fill = redBrush;
            //    StateText.Text = "On";
            //}
            //else
            //{
            //    LEDStatus = 0;
            //    if (pin != null)
            //    {
            //        pin.Write(GpioPinValue.High);
            //    }
            //    LED.Fill = grayBrush;
            //    StateText.Text = "Off";
            //}
        }

        private void TurnOffLED()
        {
            if (LEDStatus == 1)
            {
                FlipLED();
            }
        }
        private void TurnOnLED()
        {
            if (LEDStatus == 0)
            {
                FlipLED();
            }
        }

        private void TurnRelay1On()
        {
            this.ftdi.DeviceConnection.WriteASCII("ON1");
        }

        private void TurnRelay1Off()
        {
            this.ftdi.DeviceConnection.WriteASCII("OFF1");
        }

        private void TurnRelay2On()
        {
            this.ftdi.DeviceConnection.WriteASCII("ON2");
        }

        private void TurnRelay2Off()
        {
            this.ftdi.DeviceConnection.WriteASCII("OFF2");
        }

        private int LEDStatus = 0;
        private const int LED_PIN = 5;
        private GpioPin pin;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
    }
}
