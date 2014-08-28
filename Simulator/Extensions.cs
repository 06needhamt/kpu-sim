using System;
using System.Windows.Input;
using DrWPF.Windows.Data;
using KyleHughes.CIS2118.KPUSim.Assembly;
using KyleHughes.CIS2118.KPUSim.ViewModels;

namespace KyleHughes.CIS2118.KPUSim
{
    /// <summary>
    /// Extension methods used by the program
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Whether the given array contains the given index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ContainsIndex<T>(this T[] array, int index)
        {
            try
            {
                var v = array[index];
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Whether the given dictionary contains the given key
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ContainsIndex<K, V>(this ObservableDictionary<K, V> array, K index)
        {
            try
            {
                var v = array[index];
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// whether the given string contains the given index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ContainsIndex(this string array, int index)
        {
            try
            {
                var v = array[index];
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// whether the wordkind is a literal type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsLiteral(this WordKind type)
        {
            return type == WordKinds.DecimalLiteral || type == WordKinds.HexadecimalLiteral ||
                   type == WordKinds.LabelReference;
        }
        /// <summary>
        /// whether the runstate signifies an error
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsError(this RunState type)
        {
            return type == RunState.CompileError || type == RunState.RunError;
        }
    }
}