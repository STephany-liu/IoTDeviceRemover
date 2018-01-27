using Microsoft.Azure.Devices;
using System;

namespace IoTSimulatedDeviceRemover
{
    class DeviceRemoverProgram
    {
        static RegistryManager registryManager;
        static string connectionString = "your IoThub connection string";
        static void Main(string[] args)
        {
            bool wantRemove = true;
            while (wantRemove)
            {
                RemoveDevice().Wait();
                Console.WriteLine("Do you want to remove other devices? y/n");
                wantRemove = Console.ReadLine().Contains("y");
            }
        }

        private static async System.Threading.Tasks.Task RemoveDevice()
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            Console.WriteLine("Please type in the pattern. If a device's id contain this, it will be removed.");
            string pattern = Console.ReadLine(); 

            // TODO: Something wrong here. After deleting devices the total number of devices is not updated.
            RegistryStatistics stat = await registryManager.GetRegistryStatisticsAsync();
            long total_num_device = stat.TotalDeviceCount;
            Console.WriteLine("The total number of devices in this IoThub is " + total_num_device +". How many devices do you want to remove?");

            string amount_devices_tobe_removed = Console.ReadLine();
            int num_devices_tobe_removed = Convert.ToInt32(amount_devices_tobe_removed);
            while (num_devices_tobe_removed > total_num_device)
            {
                Console.WriteLine("# of to be removed device is larger than the total number of devices connected to the IoThub. Please provide a new number");
                num_devices_tobe_removed = Convert.ToInt16( Console.ReadLine() );
            }

            int num_retrived_device = 0;
            int num_removed_device = 0;
            while ((num_removed_device < num_devices_tobe_removed ) && (num_retrived_device < total_num_device))
            {
                num_retrived_device += 100;
                var devices = await registryManager.GetDevicesAsync(num_retrived_device);
                foreach (var item in devices)
                {
                    if (item.Id.Contains(pattern))
                    {
                        await registryManager.RemoveDeviceAsync(item);
                        Console.WriteLine("Divice: " + item.Id + " has been removed");
                        num_removed_device++;
                        if(num_removed_device == num_devices_tobe_removed) { break; }
                    }
                }
            }
            Console.WriteLine("Total number of removed device is " + num_removed_device);
            Console.ReadLine();
        }
    }
}
