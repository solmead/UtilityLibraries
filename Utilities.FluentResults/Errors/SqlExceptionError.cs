using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FluentResults.Errors
{
    public class SqlExceptionError : Error
    {
        public SqlExceptionError(string message, int errorCode, int lineNumber, string source, string procedure) :
           base($"A SqlException was thrown: {message}")
        {
            Metadata.Add("Exception Message", message ?? "");
            Metadata.Add("Error Code:", errorCode);
            Metadata.Add("LineNumber", lineNumber);
            Metadata.Add("Source", source ?? "");
            Metadata.Add("Procedure", procedure ?? "");
        }
    }
}
