using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kafka.Client.Producers.Partitioning;

namespace TestPartition
{
    class MyPartitioner : IPartitioner<String>
    {
        public MyPartitioner()
        {
        }


        public int Partition(String key, int a_numPartitions)
	   {
		int partition = 0;
		int offset = key.LastIndexOf('.');
		if (offset > 0)
		{
            partition = (Convert.ToInt32(key.Substring(offset + 1))) % a_numPartitions;
			
		}
		return partition;
	}

    }
}
