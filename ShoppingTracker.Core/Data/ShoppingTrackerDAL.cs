using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using System;
using System.Linq;
using ShoppingTracker.Core.Config;
using ShoppingTracker.Core.Models;
using System.Collections.Generic;

namespace ShoppingTracker.Core.Data
{
    public class ShoppingTrackerDAL
    {
        public const string SHOPPING_FAVORITES_TABLE = "shopping_favorites";
        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        public static async Task<CloudTable> CreateTableAsync(string tableName, string storageConnectionString)
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }

            Console.WriteLine();
            return table;
        }

        public static async Task<CloudTable> CreateTableAsync(string tableName)
        {
            string storageConnectionString = AppSettings.LoadAppSettings().StorageConnectionString;

            return await CreateTableAsync(tableName, storageConnectionString);
        }

        public static async Task<ShoppingFavorite> InsertOrMergeShoppingFavoriteAsync(CloudTable table, ShoppingFavorite entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                ShoppingFavorite insertedCustomer = result.Result as ShoppingFavorite;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedCustomer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        } 
        
        public static async Task<List<ShoppingFavorite>> GetShoppingFavoriteAsync(CloudTable table)
        {
            try
            {
                TableContinuationToken token = null;
                var entities = new List<ShoppingFavorite>();
                do
                {
                    var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<ShoppingFavorite>(), token);
                    entities.AddRange(queryResult.Results);
                    token = queryResult.ContinuationToken;
                } while (token != null);
                return entities.OrderByDescending(f => f.FavoritesDate).ToList();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task<ShoppingFavorite> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<ShoppingFavorite>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                ShoppingFavorite favorite = result.Result as ShoppingFavorite;
                if (favorite != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}", favorite.PartitionKey, favorite.RowKey, favorite.Name);
                }

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return favorite;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task<double?> DeleteFavoriteAsync(CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                if (rowKey == null && partitionKey == null)
                {
                    throw new ArgumentNullException("rowKey partitionKey");
                }
                Console.WriteLine(rowKey,partitionKey);
                var favoriteToDelete = await RetrieveEntityUsingPointQueryAsync(table,partitionKey,rowKey);
                TableOperation deleteOperation = TableOperation.Delete(favoriteToDelete);
                TableResult result = await table.ExecuteAsync(deleteOperation);

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
                }

                return result.RequestCharge;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}