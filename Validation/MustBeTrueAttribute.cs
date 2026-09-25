using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GiftOfTheGivers.Validation
{
    /// <summary>
    /// Validates that a checkbox is ticked. Uses the native "required" rule
    /// client-side, which handles checkboxes correctly.
    /// </summary>
    public class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object? value) => value is true;

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-required",
                ErrorMessage ?? "You must select this checkbox.");
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}