using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JDBudgetPlanner.Helpers
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                var trimmedvalue = valueResult.AttemptedValue.Trim();
                actualValue = Decimal.Parse(trimmedvalue, CultureInfo.CurrentCulture);

                string decimalSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string thousandSep = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

                thousandSep = Regex.Replace(thousandSep, @"\u00A0", " "); //used for culture with non breaking space thousand separator

                if (trimmedvalue.IndexOf(thousandSep) >= 0)
                {
                    //check validity of grouping thousand separator

                    //remove the "decimal" part if exists
                    string integerpart = trimmedvalue.Split(new string[] { decimalSep }, StringSplitOptions.None)[0];

                    //recovert double value (need to replace non breaking space with space present in some cultures)
                    string reconvertedvalue = Regex.Replace(((decimal)actualValue).ToString("N").Split(new string[] { decimalSep }, StringSplitOptions.None)[0], @"\u00A0", " ");
                    //if are the same, it is a valid number
                    if (integerpart == reconvertedvalue)
                        return actualValue;
                    //if not, could be differences only in the part before first thousand separator (for example original input stirng could be +1.000,00 (example of italian culture) that is valid but different from reconverted value that is 1.000,00; so we need to make a more accurate checking to verify if input string is valid


                    //check if number of thousands separators are the same
                    int nThousands = integerpart.Count(x => x == thousandSep[0]);
                    int nThousandsconverted = reconvertedvalue.Count(x => x == thousandSep[0]);

                    if (nThousands == nThousandsconverted)
                    {
                        //check if all group are of groupsize number characters (exclude the first, because could be more than 3 (because for example "+", or "0" before all the other numbers) but we checked number of separators == reconverted number separators
                        int[] groupsize = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes;
                        bool valid = ValidateNumberGroups(integerpart, thousandSep, groupsize);
                        if (!valid)
                            throw new FormatException();

                    }
                    else
                        throw new FormatException();

                }


            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
        private bool ValidateNumberGroups(string value, string thousandSep, int[] groupsize)
        {
            string[] parts = value.Split(new string[] { thousandSep }, StringSplitOptions.None);
            for (int i = parts.Length - 1; i > 0; i--)
            {
                string part = parts[i];
                int length = part.Length;
                if (groupsize.Contains(length) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}