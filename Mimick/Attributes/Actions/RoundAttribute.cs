using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated property, parameter or return value should be rounded to a nearest value. When applied to a method
    /// all parameters will be rounded where supported.
    /// </summary>
    /// <remarks>
    /// This attribute will work against numeric types. If the value is <c>null</c> or the value type is not supported then the attribute will not perform any rounding operation.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    [CompilationOptions(CopyArguments = true, Scope = AttributeScope.MultiSingleton)]
    [DebuggerStepThrough]
    public class RoundAttribute : ActionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundAttribute"/> class.
        /// </summary>
        public RoundAttribute() : this(0, MidpointRounding.AwayFromZero)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundAttribute" /> class.
        /// </summary>
        /// <param name="decimals">The number of decimal places to round to.</param>
        public RoundAttribute(int decimals) : this(decimals, MidpointRounding.AwayFromZero)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundAttribute" /> class.
        /// </summary>
        /// <param name="rounding">The rounding method to use.</param>
        public RoundAttribute(MidpointRounding rounding) : this(0, rounding)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundAttribute"/> class.
        /// </summary>
        /// <param name="decimals">The number of decimal places to round to.</param>
        /// <param name="rounding">The rounding method to use.</param>
        public RoundAttribute(int decimals, MidpointRounding rounding)
        {
            Decimals = decimals;
            Rounding = rounding;
        }

        #region Properties

        /// <summary>
        /// Gets the number of decimal places which should be rounded to. If not supplied, this defaults to zero.
        /// </summary>
        public int Decimals
        {
            get;
        }

        /// <summary>
        /// Gets the rounding method which should be used. If not supplied, this defaults to <see cref="MidpointRounding.AwayFromZero"/>.
        /// </summary>
        public MidpointRounding Rounding
        {
            get;
        }

        /// <summary>
        /// Applies the action to the parameter, property or return value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The value type.</param>
        /// <returns>
        /// The actioned value.
        /// </returns>
        protected override object Apply(object value, Type type)
        {
            if (value == null)
                return null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                    return Math.Round((decimal)value, Decimals, Rounding);
                case TypeCode.Double:
                    return Math.Round((double)value, Decimals, Rounding);
                case TypeCode.Single:
                    return Math.Round((float)value, Decimals, Rounding);
            }

            return value;
        }

        #endregion
    }
}
