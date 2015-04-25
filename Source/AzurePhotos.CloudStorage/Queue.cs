using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzurePhotos.CloudServices
{
	/// <summary>
	/// Provides functions to interact with Queues
	/// </summary>
	public static class Queue
	{
		private static readonly Dictionary<string, CloudQueue> CloudQueues = new Dictionary<string, CloudQueue>();
		private static readonly object Lock = new object();

		/// <summary>
		/// Gets a reference to a Queue
		/// </summary>
		public static CloudQueue GetQueue(string queueName, string configurationName = "AzureBlobStorageConnectionString")
		{
			lock (Lock)
			{
				if (CloudQueues.ContainsKey(queueName))
				{
					return CloudQueues[queueName];
				}
			}
			CreateCloudQueue(queueName);
			lock (Lock)
			{
				return CloudQueues[queueName];
			}
		}

		/// <summary>
		/// Creates a Queue
		/// </summary>
		/// <param name="queueName">The name of the Queue</param>
		/// <param name="configurationName">The configuration name to use</param>
		/// <returns></returns>
		public static CloudQueue CreateCloudQueue(string queueName, string configurationName = "AzureBlobStorageConnectionString")
		{
			lock (Lock)
			{
				try
				{
					if (CloudQueues.ContainsKey(queueName))
					{
						return CloudQueues[queueName];
					}

					string configurationSetting = RoleEnvironment.IsAvailable
						? RoleEnvironment.GetConfigurationSettingValue(configurationName)
						: ConfigurationManager.AppSettings[configurationName];

					CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configurationSetting);
					CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
					CloudQueue queue = queueClient.GetQueueReference(queueName);
					queue.CreateIfNotExists();
					
					CloudQueues.Add(queueName, queue);
					return queue;

				}
				catch (WebException)
				{

					throw new WebException("he Windows Azure storage services cannot be contacted " +
						 "via the current account configuration or the local development storage tool is not running. " +
						 "Please start the development storage tool if you run the service locally!");
				}
			}
		}

		/// <summary>
		/// Adds a message to the requested queue
		/// </summary>
		/// <param name="queueName"></param>
		/// <param name="message"></param>
		/// <remarks>See: http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storage.queue.cloudqueuemessage_members.aspx </remarks>
		public static void AddMessageToQueue<T>(string queueName, T message)
		{
			var queue = GetQueue(queueName);
			queue.AddMessage(new CloudQueueMessage(ByteArraySerializer<T>.Serialize(message)));
		}

		/// <summary>
		/// Gets a message from the queue
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public static T GetMessage<T>(string queueName)
		{
			var queue = GetQueue(queueName);
			var cloudQueueMessage = queue.GetMessage();
			return ByteArraySerializer<T>.Deserialize(cloudQueueMessage.AsBytes);
		}
	}
}