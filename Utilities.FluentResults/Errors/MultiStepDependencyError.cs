using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FluentResults.Errors
{
    /// <summary>
    /// Represents an Error that is caused by other Errors.  This is usually the result of 
    /// a higher order function attempting to use data that was retrieved from a lower call but the data 
    /// needed is not present (ie a specific code out of a list of codes is not present).
    /// This can be used to provide error information of a fault where a dependancy cannot be met.
    /// 
    /// During an Update/Post/Put this usually results in a 422 - Unprocessable entity 
    /// </summary>
    public class MultiStepDependencyError : Error
    {

        public MultiStepDependencyError(string errorText, string procedure) :
            base($"A multistep dependancy error occured: {errorText} in procedure: {procedure}")
        {
            // This is probably redundant but... 
            Metadata.Add("ErrorText", errorText);
            Metadata.Add("Procedure", procedure);
        }
    }
}
