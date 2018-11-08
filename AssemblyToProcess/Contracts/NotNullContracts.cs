﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Contracts
{
    /// <summary>
    /// A class containing methods introducing the <see cref="NotNullAttribute"/>.
    /// </summary>
    public class NotNullContracts
    {
        /// <summary>
        /// Passes regardless of whether a <c>null</c> value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PassIfNull(object value)
        {

        }

        /// <summary>
        /// Throws an exception when a <c>null</c> value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfNull([NotNull] object value)
        {

        }

        /// <summary>
        /// Throws an exception when any argument has a <c>null</c> value entered.
        /// </summary>
        /// <param name="value1">The 1st value.</param>
        /// <param name="value2">The 2nd value.</param>
        [NotNull]
        public void ThrowIfAnyNull(object value1, object value2)
        {

        }
    }
}
