using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes
{
    /// <summary>
    /// A class containing methods which should be invoked before and after the constructor of the type.
    /// </summary>
    public class ConstructAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructAttributes" /> class.
        /// </summary>
        public ConstructAttributes()
        {
            if (BeforeConstructionCount == 0)
                throw new Exception();

            ConstructionCount++;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructAttributes" /> class.
        /// </summary>
        /// <param name="a">The optional first argument.</param>
        public ConstructAttributes(int a) : this(a, 1)
        {
            ConstructionCount++;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructAttributes" /> class.
        /// </summary>
        /// <param name="a">The optional first argument.</param>
        /// <param name="b">The optional second argument.</param>
        public ConstructAttributes(int a, int b)
        {
            if (BeforeConstructionCount == 0)
                throw new Exception();

            ConstructionCount++;
        }

        /// <summary>
        /// Gets or sets the count of the times the after constructor method has been called.
        /// </summary>
        public int AfterConstructionCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the count of the times the before constructor method has been called.
        /// </summary>
        public int BeforeConstructionCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the count of the times the constructor method has been called.
        /// </summary>
        public int ConstructionCount { get; set; } = 0;

        /// <summary>
        /// Called before the constructor body has been processed.
        /// </summary>
        [PreConstruct]
        private void BeforeConstructor()
        {
            BeforeConstructionCount++;
        }

        /// <summary>
        /// Called after the constructor body has been processed.
        /// </summary>
        [PostConstruct]
        private void AfterConstructor()
        {
            AfterConstructionCount++;
        }
    }
}
