﻿using System.Diagnostics;

namespace ConsoleProDEMO.Utilities
{
    public static class EventLogWriter
    {
        private const string EventLogApplicationSubSource = ".NET Runtime";
        private const int EventLogDefaultId = 1026;

        /// <summary>
        /// Writes a given message to the Application Windows EventLog.
        /// The optional parameters can be overwritten with custom values.
        /// </summary>
        /// <param name="message">message to be written as log entry</param>
        /// <param name="eventLogEntryType">overwrites the default type of Information to a custom set type</param>
        /// <param name="sourceName">Overwrites the default ".NET Runtime". Custom values have to exist, or this throws an exception.</param>
        /// <param name="eventId">Overwrites the default id of 1026 to a custom id. keeping the default is recommended, else chose an id above 1000</param>
        public static void WriteApplicationEventEntry(
            string message,
            EventLogEntryType eventLogEntryType = EventLogEntryType.Information,
            string sourceName = EventLogApplicationSubSource,
            int eventId = EventLogDefaultId)
        {
            using (EventLog eventLog = new EventLog())
            {
                eventLog.Source = sourceName;
                eventLog.WriteEntry(message, eventLogEntryType, eventId);
            }
        }
    }
}