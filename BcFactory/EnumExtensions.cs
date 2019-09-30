
using System;
using System.Collections.Generic;
using System.Linq;

namespace BcFactory
{
    /// <summary>
    /// Helper Class to simplify transformation of model enums to given useCase.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Generic helper function which filters enums and returns the result. 
        /// </summary>
        /// <param name="ignoredValues">list holding values to be filtered out.</param>
        /// <typeparam name="T">generic Enum</typeparam>
        /// <returns>collection of type T</returns>
        public static IEnumerable<T> ValuesExcept<T>(params T[] ignoredValues)
        where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Where(val => !ignoredValues.Contains(val));
        }
    }
}