package com.mhh.cloud.kafka.partition;

import kafka.producer.Partitioner;
import kafka.utils.VerifiableProperties;


public class TestPartitioner implements Partitioner {
	public TestPartitioner(VerifiableProperties props)
	{
		
	}
	
	@Override
	public int partition(Object obj, int a_numPartitions)
	{
		String key = obj.toString();
		int partition = 0;
		int offset = key.lastIndexOf('.');
		if (offset > 0)
		{
			partition = Integer.parseInt(key.substring(offset + 1)) % a_numPartitions;
			
		}
		return partition;
	}

}
