using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// Exention methods related to string objects
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a list of integers from a string of comma separated, hyphonated numbers. IE. printer pages.
        /// </summary>
        /// <param name="intChain">The int chain.</param>
        /// <param name="wildcard">The wildcard.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException"></exception>
        public static List<int> ToIntList(this string intChain, char wildcard = 'X')
        {
            List<int> resultInts = new List<int>();

            if (String.IsNullOrEmpty(intChain))
                return resultInts;

            try
            {
                string[] chunks = intChain.Split(',');
                foreach (string chunk in chunks)
                {
                    if (String.IsNullOrEmpty(chunk))
                        continue;

                    if (!chunk.Contains("-"))
                    {
                        if (chunk.Length == 1 && Char.ToUpper(chunk[0]) == Char.ToUpper(wildcard))
                            resultInts.Add(-1);
                        else
                            resultInts.Add(Int32.Parse(chunk));
                    }
                    else
                    {
                        string[] series = chunk.Split('-');

                        int start = Int32.Parse(series[0]);
                        int end = Int32.Parse(series[1]);
                        
                        if (start > end)
                        {
                            for (int i = start; i >= end; i--)
                                resultInts.Add(i);
                        }
                        else
                        {
                            for (int i = start; i <= end; i++)
                                resultInts.Add(i);
                        }
                    }
                }
            }
            catch (FormatException)
            {
                throw new FormatException(String.Format("Integer map '{0}' has invalid characters.", intChain));
            }

            return resultInts;
        }

        /// <summary>
        /// Creates comma separated, hyphonated string of numbers from a list of integers. (ie. printer pages)
        /// </summary>
        /// <param name="ints">The ints.</param>
        /// <returns></returns>
        public static string ToIntListString(this List<int> ints)
        {
            return ToIntListString(ints, 'X', false);
        }

        /// <summary>
        /// Creates comma separated, hyphonated string of numbers from a list of integers. (ie. printer pages)
        /// </summary>
        /// <param name="ints">The ints.</param>
        /// <param name="wildcard">The wildcard to represent negative integers.</param>
        /// <returns></returns>
        public static string ToIntListString(this List<int> ints, char wildcard)
        {
            return ToIntListString(ints, wildcard, true);
        }

        /// <summary>
        /// Creates comma separated, hyphonated string of numbers from a list of integers. (ie. printer pages)
        /// </summary>
        /// <param name="ints">The ints.</param>
        /// <param name="wildcard">The wildcard to represent negative integers.</param>
        /// <param name="useWildcard">if set to <c>true</c> to use wildcards.</param>
        /// <returns></returns>
        public static string ToIntListString(this List<int> ints, char wildcard, bool useWildcard)
        {
            if (ints.Count < 1)
                return "";
            
            bool broken = true;
            bool ascending = false;
            string listString = "";
            
            if (ints.Count > 1 && ints[0] < ints[1])
                ascending = true;
            
            for (int i = 0; i < ints.Count - 1; i++)
            {
                if (useWildcard && ints[i] < 0)
                {
                    listString += wildcard.ToString() + ',';
                    broken = true;
                    continue;
                }

                if (broken)
                {
                    listString += ints[i];

                    if (ints[i + 1] == ints[i] + 1)
                    {
                        broken = false;
                        ascending = true;
                        listString += '-';
                    }
                    else if (ints[i + 1] == ints[i] - 1)
                    {
                        broken = false;
                        ascending = false;
                        listString += '-';
                    }
                    else
                    {
                        listString += ',';
                    }
                }
                else
                {
                    if (!useWildcard || ints[i + 1] > -1)
                    {
                        if (ascending && ints[i + 1] == ints[i] + 1)
                            continue;

                        if (!ascending && ints[i + 1] == ints[i] - 1)
                            continue;
                    }

                    broken = true;
                    listString += ints[i].ToString() + ',';
                }
            }

            if (useWildcard && ints[ints.Count - 1] < 0)
                listString += wildcard;
            else
                listString += ints[ints.Count - 1];

            return listString;
        }
    }
}
