using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes
{
    /// <summary>
    /// A class containing methods introducing the <see cref="ReaderAttribute"/> and <see cref="WriterAttribute"/>.
    /// </summary>
    public class ReaderWriterAttributes
    {
        private List<string> values = new List<string>();

        #region Properties

        /// <summary>
        /// Gets or sets the value at the provided index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value.</returns>
        [Reader]
        [Writer]
        public string this[int index]
        {
            get => values[index];
            set => values[index] = value;
        }

        /// <summary>
        /// Gets the number of values.
        /// </summary>
        [Reader]
        public int Count => values.Count;

        #endregion

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value.</param>
        [Writer]
        public void Add([NotEmpty] string value) => values.Add(value);

        /// <summary>
        /// Gets a value at the provided index.
        /// </summary>
        /// <param name="index">The index of the value.</param>
        /// <returns>The value.</returns>
        [Reader]
        public string Get(int index) => values[index];
    }
}
