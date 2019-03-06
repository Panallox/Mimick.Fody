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
        public Variable(FieldReference field) => Reference = field ?? throw new ArgumentNullException(nameof(field));

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="local">The local.</param>
        public Variable(VariableDefinition local) => Reference = local ?? throw new ArgumentNullException(nameof(local));

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public Variable(ParameterReference parameter) => Reference = parameter ?? throw new ArgumentNullException(nameof(parameter));

        #region Properties

        /// <summary>
        /// Gets whether the variable is a field.
        /// </summary>
        public bool IsField => Reference is FieldReference;

        /// <summary>
        /// Gets whether the variable is a local.
        /// </summary>
        public bool IsLocal => Reference is VariableDefinition;

        /// <summary>
        /// Gets whether the variable is a parameter.
        /// </summary>
        public bool IsParameter => Reference is ParameterReference;

        /// <summary>
        /// Gets whether the variable requires the <see cref="Codes.This"/> value loading onto the instruction set.
        /// </summary>
        public bool IsThisNeeded => IsField && !(Reference as FieldDefinition ?? ((FieldReference)Reference).Resolve()).IsStatic;

        /// <summary>
        /// Gets the variable reference.
        /// </summary>
        public object Reference
        {
            get;
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        public TypeReference Type
        {
            get
            {
                if (IsField)
                    return ((FieldReference)Reference).FieldType;
                if (IsLocal)
                    return ((VariableDefinition)Reference).VariableType;
                if (IsParameter)
                    return ((ParameterReference)Reference).ParameterType;
                throw new InvalidCastException();
            }
        }

        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="FieldReference"/> to <see cref="Variable"/>.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Variable(FieldReference field) => new Variable(field);

        /// <summary>
        /// Performs an implicit conversion from <see cref="VariableDefinition"/> to <see cref="Variable"/>.
        /// </summary>
        /// <param name="local">The local.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Variable(VariableDefinition local) => new Variable(local);

        /// <summary>
        /// Performs an implicit conversion from <see cref="ParameterReference"/> to <see cref="Variable"/>.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Variable(ParameterReference parameter) => new Variable(parameter);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Variable"/> to <see cref="FieldDefinition"/>.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        /// <exception cref="System.InvalidCastException"></exception>
        public static implicit operator FieldReference(Variable var) => var.Reference as FieldReference ?? throw new InvalidCastException();

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
        public static implicit operator ParameterReference(Variable var) => var.Reference as ParameterReference ?? throw new InvalidCastException();
    }
}
