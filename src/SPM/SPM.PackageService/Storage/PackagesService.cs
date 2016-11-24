﻿using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechSmith.Hyde;

namespace SPM.PackageService.Storage
{
    public class PackagesService
    {
        private readonly string connectionString;

        private const string PackagesVersionsTableName = "packageversion";
        private const string PackagesTableName = "packages";

        public PackagesService(IOptions<Startup.ConnectionStrings> azureConnectionString)
        {
            this.connectionString = azureConnectionString.Value.AzureStorage;
        }

        private TechSmith.Hyde.Table.AzureTableStorageProvider GetStorage()
        {
            var account = new ConnectionStringCloudStorageAccount(connectionString);
            var storage = new TechSmith.Hyde.Table.AzureTableStorageProvider(account);
            return storage;
        }

        public async Task<PackageVersion> GetLastPackageVersion(string packageName)
        {
            var storage = GetStorage();
            var packageVersions = await storage.CreateQuery<PackageVersion>(PackagesVersionsTableName).PartitionKeyEquals(packageName).Top(1).Async();
            if (packageVersions.Any())
                return packageVersions.First();
            return null;
        }

        internal async Task AddVersion(string packageName, string version, string fileUrl)
        {
            var storage = GetStorage();

            storage.Add(PackagesVersionsTableName, new PackageVersion(packageName, version, fileUrl));

            var package = await storage.GetAsync<Package>(PackagesTableName, "Packages", packageName);

            if (package == null)
                package = new Package(packageName);

            package.LastVersion = version;

            storage.Upsert(PackagesTableName, package);

            await storage.SaveAsync();
        }

        public async Task<string> AddFile(string packageName, string version, Stream file)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(packageName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(packageName + "-" + version);
            await blob.UploadFromStreamAsync(file);

            return blob.Uri.ToString();
        }
    }
}
