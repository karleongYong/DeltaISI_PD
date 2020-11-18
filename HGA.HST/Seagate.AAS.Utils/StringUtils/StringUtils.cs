using System;

namespace Seagate.AAS.Utils
{
	/// <summary>
	/// Summary description for StringUtils.
	/// </summary>
	public class StringUtils
	{

        public static string AddCamelSpace(string camelString)
        {
            string output = camelString;
            int index = 1; // start at second character

            while (index < output.Length-1)
            {
                if (Char.IsUpper(output, index) && (Char.IsLower(output, index-1) || Char.IsLower(output, index+1)))
                {
                    output = output.Insert(index, " ");
                    index ++;
                }
    
                index++;
            }

            return output;

        }

	}
}
