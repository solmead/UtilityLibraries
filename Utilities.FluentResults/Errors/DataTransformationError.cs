using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FluentResults.Errors
{
    public class DataTransformationError : Error
    {
        public DataTransformationError(string errorText) :
            base($"A data transformation error occured: {errorText}")
        {
            // This is probably redundant but... 
            Metadata.Add("ErrorText", errorText);
        }
    }
}
