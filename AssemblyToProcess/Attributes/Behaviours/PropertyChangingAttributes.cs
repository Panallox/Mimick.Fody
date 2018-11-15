using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Behaviours
{
    /// <summary>
    /// A class expected to implement the <see cref="INotifyPropertyChanging"/> interface.
    /// </summary>
    [PropertyChanging]
    public class PropertyChangingAttributes
    {
        /// <summary>
        /// Gets or sets a value which should not raised the property changing event.
        /// </summary>
        [IgnoreChanging]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value which should raise the property changing event.
        /// </summary>
        public string Text { get; set; }
    }
}
