using System.Collections.Generic;
using NServiceBus.SequenceGate.EntityFramework;

namespace NServiceBus.SequenceGate
{ 
    /// <summary>
    /// Interface for parsing messages
    /// </summary>
    internal interface IParser
    {
        /// <summary>
        /// Parses and unpacks the message in the pipeline into format suitable for processing in the sequence gate.
        /// further processing.
        /// </summary>
        /// <param name="message">Parsed data</param>
        /// <returns></returns>
        ParsedMessage Parse(object message);
    }
}