using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{ 
    /// <summary>
    /// Interface for parsing messages
    /// </summary>
    internal interface IParser
    {
        /// <summary>
        /// Parses and unpacks the message in the pipeline into repository format for
        /// further processing.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageMetaData"></param>
        /// <returns></returns>
        IEnumerable<GateData> Parse(object message, SequenceGateMessage messageMetaData);
    }
}