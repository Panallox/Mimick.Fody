using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Values
{
    /// <summary>
    /// A class representing a node of a value.
    /// </summary>
    internal sealed class Node
    {
        /// <summary>
        /// The next node in the sequence.
        /// </summary>
        public Node Next;

        /// <summary>
        /// The previous node in the sequence.
        /// </summary>
        public Node Previous;

        /// <summary>
        /// The parent container node which owns this node.
        /// </summary>
        public Node Parent;

        /// <summary>
        /// The type of content that the node contains.
        /// </summary>
        public NodeType Type;

        /// <summary>
        /// The content of the node, boxed to an object value to be unboxed later.
        /// </summary>
        public object Value;

        /// <summary>
        /// The indicator to whether the node has been resolved previously.
        /// </summary>
        public bool IsResolved;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Type == NodeType.Group)
                return string.Join(",", (Value as IEnumerable<Node>).Select(a => a.ToString()));

            return Value.ToString();
        }
    }

    /// <summary>
    /// Indicates the type of content that a node represents.
    /// </summary>
    internal enum NodeType
    {
        /// <summary>
        /// The node contains a constant value.
        /// </summary>
        Constant,

        /// <summary>
        /// The node contains child nodes contained within a group.
        /// </summary>
        Group,

        /// <summary>
        /// The node contains a symbol indicating a logical operation.
        /// </summary>
        Symbol,

        /// <summary>
        /// The node contains a variable value which must be resolved during evaluation.
        /// </summary>
        Variable,
    }
}
