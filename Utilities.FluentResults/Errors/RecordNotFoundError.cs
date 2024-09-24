using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FluentResults.Errors
{
    public class RecordNotFoundError : Error
    {

        public RecordNotFoundError(string id, string fieldName) :
            base($"No record was found for id: {id} in fieldname: {fieldName}")
        {
            Metadata.Add("Id", id);
            Metadata.Add("FieldName", fieldName);
        }
    }
}
