using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FluentResults.Errors
{
    public class InvalidRequestData : Error
    {
        public InvalidRequestData(string fieldName, string value) :
           base($"Invalid Request Data for field name: {fieldName} with value: {value}")
        {
            // This is probably redundant but... 
            Metadata.Add("Field Name", fieldName);
            Metadata.Add("Value", value);
        }
    }
}
