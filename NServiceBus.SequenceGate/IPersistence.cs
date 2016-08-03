using System.Collections.Generic;
using NServiceBus.SequenceGate.EntityFramework;

namespace NServiceBus.SequenceGate
{
    internal interface IPersistence
    {
        /// <summary>
        /// Registers the new objects
        /// Updates existing objects that are newer
        /// Returns the object ids to dismiss, the ones that already has newer ones seen in the Db.
        /// </summary>
        /// <param name="parsedMessage">The parsing result</param>
        /// <returns>The object ids to dismiss</returns>
        List<string> Register(ParsedMessage parsedMessage);
    }
}
