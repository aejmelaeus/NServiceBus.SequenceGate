﻿using System;
using System.Reflection;

namespace NServiceBus.SequenceGate.Repository
{
    /// <summary>
    /// A member message in the Sequence Gate.
    /// </summary>
    public class SequenceGateMember
    {
        /// <summary>
        /// The type of the message
        /// </summary>
        public Type MessageType { get; set; }
        /// <summary>
        /// The property of the object id
        /// </summary>
        public string ObjectIdPropertyName { get; set; }
        /// <summary>
        /// The time stamp property of the message
        /// </summary>
        public string TimeStampPropertyName { get; set; }
    }
}
