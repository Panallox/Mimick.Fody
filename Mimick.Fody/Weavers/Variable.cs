using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody.Weavers
{
    /// <summary>
    /// A variable class representing a storage reference, such as a local variable, field or argument.
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        public Variable(FieldDefinition field) => Reference = field;

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="local">The local.</param>
        public Variable(VariableDefinition local) => Reference = local;

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public Variable(ParameterDefinition parameter) => Reference = parameter;

        #region Properties

        /// <summary>
        /// Gets whether the variable is a field.
        /// </summary>
        public bool IsField => Reference is FieldDefinition;

        /// <summary>
        /// Gets whether the variable is a local.
        /// </summary>
        public bool IsLocal => Reference is VariableDefinition;

        /// <summary>
        /// Gets whether the variable is a parameter.
        /// </summary>
        public bool IsParameter => Reference is ParameterDefinition;

        /// <summary>
        /// Gets the variable reference.
        /// </summary>
        public object Reference
        {
            get;
        }

        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="Variable"/> to <see cref="FieldDefinition"/>.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        /// <exception cref="System.InvalidCastException"></exception>
        public static implicit operator FieldDefinition(Variable var) => var.Reference as FieldDefinition ?? throw new InvalidCastException();

        /// <summary>
        /// Performs an implicit conversion from <see cref="Variable"/> to <see cref="VariableDefinition"/>.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        /// <exception cref="System.InvalidCastException"></exception>
        public static implicit operator VariableDefinition(Variable var) => var.Reference as VariableDefinition ?? throw new InvalidCastException();

        /// <summary>
        /// Performs an implicit conversion from <see cref="Variable"/> to <see cref="ParameterDefinition"/>.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        /// <exception cref="System.InvalidCastException"></exception>
        public static implicit operator ParameterDefinition(Variable var) => var.Reference as ParameterDefinition ?? throw new InvalidCastException();
    }
}
