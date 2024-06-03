using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eBauGISTriageApi.Helper.Functions
{
    /// <summary>
    /// This file contains helper functions for the rules engine.
    /// Static functions defined here can be used inside the JSON files.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Checks if a result is not null.
        /// </summary>
        /// <param name="value">The parameter to check.</param>
        /// <returns>True: value is not null, false: value is null.</returns>
        public static bool ResultExists(JToken value)
        {
            return value != null;
        }

        /// <summary>
        /// Converts a JArray to a string.
        /// </summary>
        /// <param name="array">Accepts JTokens as parameter. However, the token must be a JArray.</param>
        /// <returns>A string with the array data delimited by a comma.</returns>
        /// <exception cref="Exception">Is thrown if array is null or array is not JArray.</exception>
        public static string ConvertArrayToString(JToken array)
        {
            if (array is null)
            {
                throw new Exception("Parameter value for ConvertArrayToString() cannot be null.");
            }

            if (array is not JArray)
            {
                throw new Exception("Parameter value for ConvertArrayToString() must be an array.");
            }

            string result = String.Join(", ", array.Select(i => i.ToString()));
            return result;
        }

        /// <summary>
        /// Checks if any value of a JToken starts with the inputString.
        /// </summary>
        /// <param name="token">The dictionary JToken</param>
        /// <param name="inputString">The string to test.</param>
        /// <returns>True if some value starts with the inputString, otherwise false.</returns>
        /// <exception cref="Exception">Is thrown if token/inputString is null or token is not a dictionary.</exception>
        public static bool CheckDictionaryValuesStartWith(JToken token, string inputString)
        {
            if (token is null)
            {
                throw new Exception("First parameter value for CheckDictionaryValuesStartWith() cannot be null.");
            }

            if (inputString is null)
            {
                throw new Exception("Second parameter value for CheckDictionaryValuesStartWith() cannot be null.");
            }

            var dictionary = token.ToObject<Dictionary<string, List<string>>>();

            if (dictionary is null)
            {
                throw new Exception("JToken (first parameter) is not a dictionary.");
            }

            foreach (var valuesList in dictionary.Values)
            {
                foreach (var value in valuesList)
                {
                    if (value.StartsWith(inputString))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parses an integer value from a JToken.
        /// </summary>
        /// <param name="token">The JToken to parse.</param>
        /// <returns>The parsed integer value.</returns>
        /// <exception cref="Exception">Thrown when the <paramref name="token"/> is null 
        /// or a JValue object, or when an error occurs during parsing.</exception>
        public static int ParseIntegerJToken(JToken token)
        {
            if (token is null)
            {
                throw new Exception("First parameter value for ParseIntegerJToken() cannot be null.");
            }

            try
            {
                return token.Value<int>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while parsing the integer value from the JToken.", ex);
            }
        }
    }
}
