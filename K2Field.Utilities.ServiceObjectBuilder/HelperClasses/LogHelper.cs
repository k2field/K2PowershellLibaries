using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2Field.Utilities.ServiceObjectBuilder.HelperClasses
{
    public static class LogHelper
    {
        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        public static void LogMessage(string logMessage)
        {
            Console.WriteLine(logMessage);
        }

        /// <summary>
        /// Logs a step.
        /// </summary>
        /// <param name="StepNum">The step number</param>
        /// <param name="logMessage">The log message.</param>
        public static void LogStep(int StepNum, string logMessage)
        {
            LogMessage("\n" + StepNum.ToString() + ". " + logMessage);
        }

        /// <summary>
        /// Logs a sub step.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        public static void LogSubStep(string logMessage)
        {
            LogMessage("   -- " + logMessage);
        }

        /// <summary>
        /// Logs info for a sub step.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        public static void LogSubStepInfo(string logMessage)
        {
            LogMessage("      " + logMessage);
        }


    }
}
