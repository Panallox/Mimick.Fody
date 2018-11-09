using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing helper methods for <see cref="Type"/> operations.
    /// </summary>
    public static class TypeHelper
    {
        public static readonly Type Int8Array = typeof(sbyte[]);
        public static readonly Type Int8 = typeof(sbyte);
        public static readonly Type Int16 = typeof(short);
        public static readonly Type Int32 = typeof(int);
        public static readonly Type Int64 = typeof(long);
        public static readonly Type Nullable = typeof(Nullable<>);
        public static readonly Type String = typeof(string);
        public static readonly Type UInt8 = typeof(byte);
        public static readonly Type UInt8Array = typeof(byte[]);
        public static readonly Type UInt16 = typeof(ushort);
        public static readonly Type UInt32 = typeof(uint);
        public static readonly Type UInt64 = typeof(ulong);

        /// <summary>
        /// Converts a provided value into the requested type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="InvalidCastException">If the value cannot be converted.</exception>
        public static object Convert(string value, Type type)
        {
            if (value == null)
                return type.IsValueType ? Activator.CreateInstance(type) : null;

            if (Nullable.IsAssignableFrom(type))
                type = type.GenericTypeArguments.First();

            var code = Type.GetTypeCode(type);

            if (code == TypeCode.DBNull)
                return DBNull.Value;

            if (code == TypeCode.Empty)
                return null;

            if (code == TypeCode.String)
                return value;

            var ex = new InvalidCastException($"Cannot convert '{value}' to '{type.FullName}'");

            try
            {
                if (code == TypeCode.Boolean && char.IsNumber(value[0]) && int.TryParse(value, out var int32))
                    return int32 > 0;

                if (code != TypeCode.Object)
                {
                    var converter = TypeDescriptor.GetConverter(type);
                    return converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
                }
            }
            catch
            {

            }

            throw ex;
        }

        /// <summary>
        /// Gets a default value for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value.</returns>
        public static object Default(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return default(bool);
                case TypeCode.Byte:
                    return default(byte);
                case TypeCode.Char:
                    return default(char);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.DBNull:
                    return default(DBNull);
                case TypeCode.Decimal:
                    return default(decimal);
                case TypeCode.Double:
                    return default(double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return default(short);
                case TypeCode.Int32:
                    return default(int);
                case TypeCode.Int64:
                    return default(long);
                case TypeCode.Object:
                    return default(object);
                case TypeCode.SByte:
                    return default(sbyte);
                case TypeCode.Single:
                    return default(float);
                case TypeCode.String:
                    return default(string);
                case TypeCode.UInt16:
                    return default(ushort);
                case TypeCode.UInt32:
                    return default(uint);
                case TypeCode.UInt64:
                    return default(ulong);
                default:
                    throw new ArgumentException($"Cannot get default value of '{type.FullName}'");
            }
        }
    }
}
