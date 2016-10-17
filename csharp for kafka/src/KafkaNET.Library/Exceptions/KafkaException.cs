﻿/**
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Kafka.Client.Exceptions
{
    using Kafka.Client.Utils;
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// A wrapping of an error code returned from Kafka.
    /// </summary>
    public class KafkaException : Exception
    {
        public KafkaException()
        {
            ErrorCode = (short)ErrorMapping.NoError;
        }

        public KafkaException(string message) : base(message)
        {
            ErrorCode = (short)ErrorMapping.UnknownCode;
        }

        /// <summary>
        /// Initializes a new instance of the KafkaException class.
        /// </summary>
        /// <param name="errorCode">The error code generated by a request to Kafka.</param>
        public KafkaException(short errorCode) : base(GetMessage(errorCode))
        {
            ErrorCode = errorCode;
        }

        public KafkaException(ErrorMapping errorCode)
            : base(GetMessage((short)errorCode))
        {
            ErrorCode = (short)errorCode;
        }

        public KafkaException(string message, ErrorMapping errorCode)
            : base(message + GetMessage((short)errorCode))
        {
            ErrorCode = (short)errorCode;
        }

        /// <summary>
        /// Gets the error code that was sent from Kafka.
        /// </summary>
        public short ErrorCode { get; private set; }

        /// <summary>
        /// Gets the message for the exception based on the Kafka error code.
        /// </summary>
        /// <param name="errorCode">The error code from Kafka.</param>
        /// <returns>A string message representation </returns>
        public static string GetMessage(short errorCode)
        {
            ErrorMapping error;
            if (Enum.TryParse(errorCode.ToString(CultureInfo.InvariantCulture), out error))
            {
                return GetMessage(error);
            }

            return "Unknown Error.";
        }

        public static string GetMessage(ErrorMapping error)
        {
            switch (error)
            {
                case ErrorMapping.NoError:
                    return "NoError";
                case ErrorMapping.OffsetOutOfRangeCode:
                    return "Offset out of range.";
                case ErrorMapping.InvalidMessageCode:
                    return "Invalid message.";
                case ErrorMapping.MessagesizeTooLargeCode:
                    return "Message size too large.";
                case ErrorMapping.UnknownTopicOrPartitionCode:
                    return "Unknown topic or partition.";
                case ErrorMapping.InvalidFetchSizeCode:
                    return "Invalid fetch size passed.";
                case ErrorMapping.LeaderNotAvailableCode:
                    return "Leader not found for given topic/partition.";
                case ErrorMapping.NotLeaderForPartitionCode:
                    return "This error is thrown if the client attempts to send messages to a replica that is not the leader for some partition. It indicates that the clients metadata is out of date.";
                case ErrorMapping.RequestTimedOutCode:
                    return "This error is thrown if the request exceeds the user-specified time limit in the request.";
                case ErrorMapping.BrokerNotAvailableCode:
                    return "This is not a client facing error and is used mostly by tools when a broker is not alive.";
                case ErrorMapping.ReplicaNotAvailableCode:
                    return "If replica is expected on a broker, but is not.";
                case ErrorMapping.StaleControllerEpochCode:
                    return "Internal error code for broker-to-broker communication.";
                case ErrorMapping.OffsetMetadataTooLargeCode:
                    return "The server has a configurable maximum message size to avoid unbounded memory allocation. This error is thrown if the client attempt to produce a message larger than this maximum.";
                case ErrorMapping.StaleLeaderEpochCode:
                    return "StaleLeaderEpochCode";
                case ErrorMapping.OffsetsLoadInProgressCode:
                    return "The broker returns this error code for an offset fetch request if it is still loading offsets (after a leader change for that offsets topic partition).";
                case ErrorMapping.ConsumerCoordinatorNotAvailableCode:
                    return "The broker returns this error code for consumer metadata requests or offset commit requests if the offsets topic has not yet been created.";
                case ErrorMapping.NotCoordinatorForConsumerCode:
                    return "The broker returns this error code if it receives an offset fetch or commit request for a consumer group that it is not a coordinator for.";
            }

            return "Unknown Error.";
        }

        public KafkaException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
