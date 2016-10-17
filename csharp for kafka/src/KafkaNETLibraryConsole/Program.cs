// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using Kafka.Client.Producers;
using Kafka.Client.Cfg;
using System;
using System.Collections.Generic;
using Kafka.Client.Helper;
using TestPartition;
using Kafka.Client.Consumers;
using Kafka.Client.Requests;
using Kafka.Client.Responses;
using Kafka.Client.Messages;
using Kafka.Client.Serialization;
using System.Threading;
namespace KafkaNET.Library.Examples
    
{
    class Program
    {
        static void Main(string[] args)
        {           
            TestProducer();
            TestConsumer3();
            
        }

        static void TestProducer()
        {
            var brokerConfig = new BrokerConfiguration()
            {
                BrokerId = 0,
                Host = "localhost",
                Port = 9092
            };
            var config = new ProducerConfiguration(new List<BrokerConfiguration> {brokerConfig});

            config.PartitionerClass = "Kafka.Client.Producers.Partitioning.MyPartitioner";
            config.SerializerClass = "Kafka.Client.Serialization.StringEncoder";

            Producer<String, String> kafkaProducer = new Producer<String, String>(config);

            long events = 100;
            for (long nEvents = 0; nEvents < events; nEvents++)
            {

                DateTime dt = DateTime.Now;
                long tick = DateTime.Now.Ticks;
                Random rnd = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                String runtime = dt.ToString();
                String ip = "192.168.253." + rnd.Next(0, 255);
                String msg = nEvents + "-->" + runtime + ",test example" + "-->" + ip;
                ProducerData<String, String> data = new ProducerData<String, String>("mhh_page_visits", ip, msg);
                kafkaProducer.Send(data);
            }
        }

        //balanced consumer
        static void TestConsumer2()
        {
            String topic = "mhh_page_visits";
            int partition = 0;
            String clientName = "Client_" + "mhh_page_visits" + "_" + partition;

            ConsumerConfiguration config = new ConsumerConfiguration
            {
                AutoCommit = true,
                GroupId = "first",
                ConsumerId = clientName,
                MaxFetchBufferLength = 10000,
                FetchSize = 1000000,
                AutoOffsetReset = OffsetRequest.LargestTime,
                NumberOfTries = 100,
                ZooKeeper = new ZooKeeperConfiguration("localhost:2181", 30000, 30000, 2000)
            };

            ZookeeperConsumerConnector balancedConsumer = new ZookeeperConsumerConnector(config, true);
            
            Dictionary<String, int> topicCountDict = new Dictionary<String, int> { };
            topicCountDict[topic] = 1;
            IDictionary<string, IList<KafkaMessageStream<String>>> streams = balancedConsumer.CreateMessageStreams(topicCountDict, new StringDecoder());
  
            // start consuming stream
            KafkaMessageStream<String> kafkaMessageStream = streams[topic][partition];
            Console.WriteLine("===========================================================");

            var cancelTokenSource = new CancellationTokenSource(30);
            foreach(String message in kafkaMessageStream.GetCancellable(cancelTokenSource.Token))        
                Console.WriteLine(message);          
       }

        //SimpleManager
        static void TestConsumer3()
        {
            var managerConfig = new KafkaSimpleManagerConfiguration()
            {
                Zookeeper = "localhost:2181"
            };
            var m_consumerManager = new KafkaSimpleManager<int, Kafka.Client.Messages.Message>(managerConfig);

            int partition = 0;
            String topic = "mhh_page_visits";
            String clientName = "Client_" + "mhh_page_visits1" + "_" + partition;
            var allPartitions = m_consumerManager.GetTopicPartitionsFromZK(topic);
           
            m_consumerManager.RefreshMetadata(0, clientName, 10, topic, true);
            var partitionConsumer = m_consumerManager.GetConsumer(topic, 0);

            foreach (var message in partitionConsumer.Fetch(clientName, topic, 5, 0, 1, 10000000, 10, 5).MessageSet(topic, 0))
                  Console.WriteLine("Message: {0}, Offset: {1}", message.Message, message.MessageOffset);
       } 
    }  
}
