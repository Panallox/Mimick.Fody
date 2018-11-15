using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Behaviours
{
    /// <summary>
    /// A class expected to implement the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    [PropertyChanged]
    public class PropertyChangedAttributes
    {
        /// <summary>
        /// Gets or sets a value which should not raised the property changed event.
        /// </summary>
        [IgnoreChange]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value which should raise the property changed event.
        /// </summary>
        public string Text { get; set; }
    }
}
