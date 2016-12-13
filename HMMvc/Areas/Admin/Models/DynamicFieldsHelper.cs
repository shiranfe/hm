using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Models
{
    public class DynamicFieldsHelper
    {

        internal static List<StepGroupFieldDM> collectionToFields(FormCollection formCollection)
        {
            var fields = new List<StepGroupFieldDM>();
            int value;

            foreach (string field in formCollection)
            {
                // if (field != "JobRefubrish_StepID" && field != "JobID" )
                var subGroupField = field.Split('_');

                var fieldName = subGroupField.Length > 1 ? subGroupField[0]: field;
                if (int.TryParse(fieldName, out value) && !string.IsNullOrEmpty(formCollection[field]))
                {
                    var subId = subGroupField.Length > 1 ? Convert.ToInt32(subGroupField[1]) : (int?)null;
                    fields.Add(new StepGroupFieldDM
                    {
                        DynamicGroupFieldID = Convert.ToInt32(fieldName),
                        FieldValue = formCollection[field],
                        SubGroupID = subId
                    });
                }

            }

            return fields;
        }
    }
}