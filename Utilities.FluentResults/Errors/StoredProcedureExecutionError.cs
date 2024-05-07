using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FluentResults.Errors
{
    public class StoredProcedureExecutionError : Error
    {
        public StoredProcedureExecutionError(string storedProcedureName) :
            base($"Error calling Stored Procedure: {storedProcedureName}")
        {
            Metadata.Add("StoredProcedureName", storedProcedureName);
        }
    }

}
