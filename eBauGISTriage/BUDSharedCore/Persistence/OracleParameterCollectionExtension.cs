using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUDSharedCore.Persistence
{
    public static class OracleParameterCollectionExtension
    {
        public static void AddInputArray<T>(
           this OracleParameterCollection parameters,
           string name,
           OracleDbType dbType,
           T[] array,
           T emptyArrayValue)
        {
            parameters.Add(new OracleParameter
            {
                ParameterName = name,
                OracleDbType = dbType,
                CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                Direction = ParameterDirection.Input
            });

            // oracle does not support passing null or empty arrays.
            // so pass an array with exactly one element
            // with a predefined value and use it to check
            // for empty array condition inside the proc code
            if (array == null || array.Length == 0)
            {
                parameters[name].Value = new T[1] { emptyArrayValue };
                parameters[name].Size = 1;
            }
            else
            {
                parameters[name].Value = array;
                parameters[name].Size = array.Length;
            }
        }

        public static void AddOutputArray<T>(
           this OracleParameterCollection parameters,
           string name,
           OracleDbType dbType,
           int size)
        {
            parameters.Add(new OracleParameter
            {
                ParameterName = name,
                OracleDbType = dbType,
                CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                Direction = ParameterDirection.Output,
                Size = size
            });
        }
    }
}
