using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2Field.Utilities.ServiceObjectBuilder.HelperClasses
{
    public static class GuidHelper
    {
        /// <summary>
        /// Determines whether the specified GUID string is GUID.
        /// </summary>
        /// <param name="guidString">The GUID string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified GUID string is GUID; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGuid(string guidString)
        {
            bool bResult = false;
            try
            {
                Guid g = new Guid(guidString);
                bResult = true;
            }
            catch
            {
                bResult = false;
            }

            return bResult;
        }
    }
}
