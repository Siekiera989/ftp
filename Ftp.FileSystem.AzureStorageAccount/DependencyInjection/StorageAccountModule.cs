using Autofac;
using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Ftp.Core.Interfaces.Settings;

namespace Ftp.FileSystem.AzureStorageAccount.DependencyInjection;

public class StorageAccountModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            const string containerName = "firmware";
            var settings = c.Resolve<IStorageAccountSettings>();

            return new BlobContainerClient(settings.FirmwareStorageAccountConnectionString, containerName);
        });

        builder.Register(c =>
        {
            const string containerName = "firmware";

            var settings = c.Resolve<IStorageAccountSettings>();

            var _dataLakeServiceClient = new DataLakeServiceClient(settings.FirmwareStorageAccountConnectionString);

            return _dataLakeServiceClient.GetFileSystemClient(containerName);
        });
    }
}
