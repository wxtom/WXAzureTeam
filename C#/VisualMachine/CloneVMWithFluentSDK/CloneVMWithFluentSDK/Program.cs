using System;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace CloneVMWithFluentSDK
{
    class Program
    {   
        //clone vm from the existing vm

        static void Main(string[] args)
        {  
            var resourcegroup = "Resource Group";
            var vmName = "Visual Machine Name";
            var location = "Resource Loaction";
            var userName = "Visual Machine User Name";
            var passsword = "Visual Machine Password";
            var credentials = SdkContext.AzureCredentialsFactory.FromFile(@"F:\AzureAuth.txt"); //get azure credentials from file
            var newVitualMachineName = "New VM Name";
            var containerName = "Container Name";
            var vhdPrefix = "VHD Prefix";
            var dnsLab = "DNS Lab";
            var storageAccount = "Storage Account Name";
            var azure = Azure
                
                .Configure()

               .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)

               .Authenticate(credentials)

               .WithDefaultSubscription();


            var vm = azure.VirtualMachines.GetByResourceGroup(resourcegroup, vmName); //get vm

            string diskuri = vm.StorageProfile.OsDisk.Vhd.Uri; //get vm vhd uri

            var key = azure.StorageAccounts.GetByResourceGroup(resourcegroup, storageAccount).GetKeys()[0]; //Get account key

            //Create disk with existing vhd

            var disk = azure.Disks.Define("New disk name")
               .WithRegion(location)
               .WithExistingResourceGroup(resourcegroup)
               .WithWindowsFromVhd(diskuri)
               .Create();

            var windowsVm = azure.VirtualMachines.Define(newVitualMachineName)

                    .WithRegion(location)

                    .WithNewResourceGroup(resourcegroup)

                    .WithNewPrimaryNetwork("10.0.0.0/28")

                    .WithPrimaryPrivateIPAddressDynamic()

                    .WithNewPrimaryPublicIPAddress(dnsLab)

                    .WithSpecializedOSDisk(disk, OperatingSystemTypes.Windows)

                    .WithSize(VirtualMachineSizeTypes.StandardA0)

                    .Create();

            Console.WriteLine($"VM {windowsVm.PowerState}:{windowsVm.PowerState}");
            Console.WriteLine($"finish creating vm...{DateTime.Now}");
            Console.ReadLine();
        }
    }
}
