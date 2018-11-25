using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody.Weavers
{
    /// <summary>
    /// An emitter class containing methods for emitting code.
    /// </summary>
    public class CodeEmitter
    {
        private Queue<TryBlock> tryBlocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeEmitter"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public CodeEmitter(MethodEmitter parent)
        {
            IL = parent.Target.Body.GetILProcessor();
            Insert = CodeInsertion.Append;
            Parent = parent;
            Position = null;
            tryBlocks = new Queue<TryBlock>();
        }

        #region Properties

        /// <summary>
        /// Get the method body.
        /// </summary>
        public MethodBody Body => Parent.Target.Body;

        /// <summary>
        /// Gets the IL processor.
        /// </summary>
        public ILProcessor IL
        {
            get;
        }

        /// <summary>
        /// Gets or sets where code should be inserted.
        /// </summary>
        public CodeInsertion Insert
        {
            get; set;
        }

        /// <summary>
        /// Gets the parent method weaver.
        /// </summary>
        public MethodEmitter Parent
        {
            get;
        }

        /// <summary>
        /// Gets or sets the current position within the code.
        /// </summary>
        public Instruction Position
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Starts a <c>catch</c> block.
        /// </summary>
        /// <param name="storage">The variable which will contain the exception.</param>
        public void Catch(Variable storage)
        {
            var block = tryBlocks.Peek();

            if (block == null)
                throw new NotSupportedException($"Cannot declare a catch block outside of a try block");

            if (block.TryStart == null)
                throw new NotSupportedException($"Cannot declare a catch block without a try");

            var position = Codes.Store(storage);
            Emit(position);
            Emit(Codes.Nop);

            block.State = 1;
            block.CatchStart = position;
        }
        
        /// <summary>
        /// Create a new label within the method.
        /// </summary>
        public Label EmitLabel() => new Label();

        /// <summary>
        /// Create a new local variable within the method.
        /// </summary>
        /// <param name="type">The variable type.</param>
        /// <param name="name">The variable name.</param>
        public Variable EmitLocal(TypeReference type, string name = null) => Parent.EmitLocal(type, name);
        
        /// <summary>
        /// Emit code within the method body.
        /// </summary>
        /// <param name="code">The code.</param>
        public CodeEmitter Emit(Instruction code)
        {
            if (code == null)
                return this;

            if (Insert != CodeInsertion.Append && Position == null)
                throw new NotSupportedException($"Cannot insert relative to a position without a position (mode is {Insert} in {Parent.Target.FullName})");
                        
            switch (Insert)
            {
                case CodeInsertion.After:
                    IL.InsertAfter(Position, code);
                    Position = code;
                    break;
                case CodeInsertion.Append:
                    IL.Append(code);
                    break;
                case CodeInsertion.Before:
                    IL.InsertBefore(Position, code);
                    break;
            }
            
            if (tryBlocks.Count > 0)
            {
                var block = tryBlocks.Peek();

                switch (block.State)
                {
                    case 0:
                        block.TryEnd = code;
                        break;
                    case 1:
                        block.CatchEnd = code;
                        break;
                    case 2:
                        block.FinallyEnd = code;
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Emit a collection of codes within the method body.
        /// </summary>
        /// <param name="codes">The codes.</param>
        public CodeEmitter Emit(IEnumerable<Instruction> codes)
        {
            if (codes == null)
                return this;

            foreach (var c in codes)
            {
                if (c != null)
                    Emit(c);
            }

            return this;
        }

        /// <summary>
        /// Closes a <c>try</c> block.
        /// </summary>
        public void EndTry()
        {
            var block = tryBlocks.Dequeue();
            var position = GetTryPosition(block);

            if (block.TryEnd == null)
                return;

            if (block.CatchEnd == null && block.CatchStart != null)
                block.CatchEnd = position;

            if (block.FinallyStart != null)
            {
                var finalize = Instruction.Create(OpCodes.Endfinally);
                Emit(Codes.Nop);
                Emit(finalize);
                block.FinallyEnd = finalize;
            }

            if (block.CatchStart != null && block.CatchEnd != null)
            {
                var catches = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    TryStart = block.TryStart,
                    TryEnd = block.CatchStart,
                    CatchType = Parent.Parent.Context.Finder.Exception,
                    HandlerStart = block.CatchStart,
                    HandlerEnd = block.CatchEnd.Next ?? block.CatchEnd,
                };

                Body.ExceptionHandlers.Add(catches);
            }

            if (block.FinallyStart != null && block.FinallyEnd != null)
            {
                var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
                {
                    TryStart = block.TryStart,
                    TryEnd = block.FinallyStart,
                    HandlerStart = block.FinallyStart,
                    HandlerEnd = block.FinallyEnd.Next ?? block.FinallyEnd
                };

                Body.ExceptionHandlers.Add(handler);
            }
        }

        /// <summary>
        /// Starts a <c>finally</c> block.
        /// </summary>
        public void Finally(Instruction leave = null)
        {
            var block = tryBlocks.Peek();

            if (block == null)
                throw new NotSupportedException($"Cannot declare a finally block outside of a try block");

            if (block.TryStart == null)
                throw new NotSupportedException($"Cannot declare a finally block without a try");

            if (block.CatchStart != null && leave != null)
            {
                block.State = -1;

                var routed = Codes.Leave(leave);
                Emit(routed);

                var index = Body.Instructions.IndexOf(block.TryStart);
                var count = Body.Instructions.Count;

                while (0 <= index && index < count)
                {
                    var i = Body.Instructions[index];

                    if (i.Operand == leave)
                        Body.Instructions[index] = Instruction.Create(OpCodes.Leave, routed);

                    if (i == block.TryEnd)
                        break;

                    index++;
                }

                for (var i = block.CatchStart; i != null; i = i.Next)
                {
                    if (i.Operand == leave)
                        i.Operand = routed;

                    if (i == block.CatchEnd)
                        break;
                }
            }

            var position = Codes.Nop;
            Emit(position);

            block.FinallyStart = position;
            block.State = 2;
        }

        /// <summary>
        /// Gets the first code within the method body where the constructors calls either the <c>base</c>
        /// constructor method, or a corresponding <c>this</c> constructor method.
        /// </summary>
        /// <returns></returns>
        public Instruction GetConstructorBaseOrThis()
        {
            var m = Parent.Target;

            if (m.IsStatic || m.IsAbstract)
                return null;

            var il = Body.Instructions;

            for (int i = 0, count = il.Count; i < count; i++)
            {
                var it = il[i];

                if (it.OpCode == OpCodes.Call ||
                    it.OpCode == OpCodes.Callvirt)
                {
                    var def = (it.Operand as MethodReference)?.Resolve();

                    if (def == null || !def.IsConstructor)
                        return null;

                    var sub = m.DeclaringType.BaseType;

                    if (def.DeclaringType.FullName == m.DeclaringType.FullName || def.DeclaringType.FullName == sub.FullName)
                        return it;

                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first code within the method body.
        /// </summary>
        /// <returns></returns>
        public Instruction GetFirst()
        {
            var instruction = Body.Instructions.FirstOrDefault();

            if (instruction == null)
            {
                instruction = Codes.Return;
                Body.Instructions.Add(instruction);
            }

            return instruction;
        }

        /// <summary>
        /// Gets the last code within the method body.
        /// </summary>
        /// <returns>A <see cref="Codes"/> value; otherwise, <c>null</c> if the body is empty.</returns>
        public Instruction GetLast()
        {
            var instruction = Body.Instructions.LastOrDefault();

            if (instruction == null)
            {
                instruction = Codes.Return;
                Body.Instructions.Add(instruction);
            }

            return instruction;
        }

        /// <summary>
        /// Gets the current code for a <c>try</c> block.
        /// </summary>
        /// <param name="block">The <c>try</c> block.</param>
        /// <returns>An <see cref="Instruction"/> value.</returns>
        private Instruction GetTryPosition(TryBlock block)
        {
            var before = Insert == CodeInsertion.Before;
            var position = Position ?? (before ? GetFirst() : GetLast());

            if (block == null || block.TryEnd == null)
                return before ? (position.Previous ?? position) : (position.Next ?? position);

            return before ? position : (position.Previous ?? position);
        }

        /// <summary>
        /// Mark the current position as the entry point for a label.
        /// </summary>
        /// <param name="label">The label.</param>
        public void Mark(Label label) => label.Mark(Position ?? GetLast());

        /// <summary>
        /// Starts a <c>try</c> block.
        /// </summary>
        /// <returns>A <see cref="TryBlock"/> value.</returns>
        public TryBlock Try()
        {
            var position = Codes.Nop;
            Emit(position);

            var block = new TryBlock { State = 0, TryStart = position };
            tryBlocks.Enqueue(block);

            return block;
        }
    }

    /// <summary>
    /// A code class representing an instruction. A code is a short form of the <see cref="Instruction"/> class.
    /// </summary>
    public static class Codes
    {
        #region Properties

        /// <summary>
        /// A duplicate code.
        /// </summary>
        public static Instruction Duplicate => Instruction.Create(OpCodes.Dup);

        /// <summary>
        /// A load array code.
        /// </summary>
        public static Instruction LoadArray => Instruction.Create(OpCodes.Ldelem_Ref);

        /// <summary>
        /// A no-operation code.
        /// </summary>
        public static Instruction Nop => Instruction.Create(OpCodes.Nop);

        /// <summary>
        /// A <c>null</c> stack load code.
        /// </summary>
        public static Instruction Null => Instruction.Create(OpCodes.Ldnull);

        /// <summary>
        /// A pop code.
        /// </summary>
        public static Instruction Pop => Instruction.Create(OpCodes.Pop);

        /// <summary>
        /// A return code.
        /// </summary>
        public static Instruction Return => Instruction.Create(OpCodes.Ret);

        /// <summary>
        /// A store in array code.
        /// </summary>
        public static Instruction StoreArray => Instruction.Create(OpCodes.Stelem_Ref);

        /// <summary>
        /// A <c>this</c> stack load code.
        /// </summary>
        /// <returns></returns>
        public static Instruction This => Instruction.Create(OpCodes.Ldarg_0);

        #endregion

        /// <summary>
        /// An address load.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns></returns>
        public static Instruction Address(Variable var)
        {
            if (var.IsLocal)
                return Instruction.Create(OpCodes.Ldloca, (VariableDefinition)var);

            return null;
        }

        /// <summary>
        /// An argument code.
        /// </summary>
        /// <param name="index">The argument index.</param>
        /// <returns></returns>
        public static Instruction Arg(int index)
        {
            switch (index)
            {
                case 0:
                    return Instruction.Create(OpCodes.Ldarg_0);
                case 1:
                    return Instruction.Create(OpCodes.Ldarg_1);
                case 2:
                    return Instruction.Create(OpCodes.Ldarg_2);
                case 3:
                    return Instruction.Create(OpCodes.Ldarg_3);
            }

            if (index >= 0 && index <= 255)
                return Instruction.Create(OpCodes.Ldarg_S, (byte)index);
            else
                return Instruction.Create(OpCodes.Ldarg, index);
        }

        /// <summary>
        /// An argument code.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static Instruction Arg(ParameterReference parameter) => Instruction.Create(OpCodes.Ldarg, parameter.Resolve());

        /// <summary>
        /// A box code.
        /// </summary>
        /// <param name="type">The type.</param>
        public static Instruction Box(TypeReference type) => type.IsValueType || type.IsGenericParameter ? Instruction.Create(OpCodes.Box, type) : null;

        /// <summary>
        /// A cast code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Instruction Cast(TypeReference type) => type.IsValueType || type.IsGenericParameter ? null : Instruction.Create(OpCodes.Castclass, type);

        /// <summary>
        /// An object creation code.
        /// </summary>
        /// <param name="constructor">The object constructor.</param>
        /// <returns></returns>
        public static Instruction Create(MethodReference constructor) => Instruction.Create(OpCodes.Newobj, constructor);

        /// <summary>
        /// An array creation code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Instruction CreateArray(TypeReference type) => Instruction.Create(OpCodes.Newarr, type);

        /// <summary>
        /// A jump instruction to the provided label when the head of the stack is <c>false</c>.
        /// </summary>
        /// <param name="label">The label.</param>
        public static Instruction IfFalse(Label label)
        {
            var code = Instruction.Create(OpCodes.Brfalse, Instruction.Create(OpCodes.Nop));
            label.Reference(code);
            return code;
        }

        /// <summary>
        /// A jump instruction to the provided label when the head of the stack is <c>true</c>.
        /// </summary>
        /// <param name="label">The label.</param>
        public static Instruction IfTrue(Label label)
        {
            var code = Instruction.Create(OpCodes.Brtrue, Instruction.Create(OpCodes.Nop));
            label.Reference(code);
            return code;
        }

        /// <summary>
        /// An initialise code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Instruction Init(TypeReference type) => type.IsValueType ? Instruction.Create(OpCodes.Initobj, type) : null;
                
        /// <summary>
        /// An integer stack load code.
        /// </summary>
        /// <param name="value">The value.</param>
        public static Instruction Int(int value)
        {
            switch (value)
            {
                case -1:
                    return Instruction.Create(OpCodes.Ldc_I4_M1);
                case 0:
                    return Instruction.Create(OpCodes.Ldc_I4_0);
                case 1:
                    return Instruction.Create(OpCodes.Ldc_I4_1);
                case 2:
                    return Instruction.Create(OpCodes.Ldc_I4_2);
                case 3:
                    return Instruction.Create(OpCodes.Ldc_I4_3);
                case 4:
                    return Instruction.Create(OpCodes.Ldc_I4_4);
                case 5:
                    return Instruction.Create(OpCodes.Ldc_I4_5);
                case 6:
                    return Instruction.Create(OpCodes.Ldc_I4_6);
                case 7:
                    return Instruction.Create(OpCodes.Ldc_I4_7);
                case 8:
                    return Instruction.Create(OpCodes.Ldc_I4_8);
            }

            if (value >= -128 && value <= 127)
                return Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)value);
            else
                return Instruction.Create(OpCodes.Ldc_I4, value);
        }

        /// <summary>
        /// A method invocation code.
        /// </summary>
        /// <param name="method">The method handle.</param>
        public static Instruction Invoke(MethodReference method) => Instruction.Create(OpCodes.Callvirt, method);

        /// <summary>
        /// A static method invocation code.
        /// </summary>
        /// <param name="method">The method handle.</param>
        public static Instruction InvokeStatic(MethodReference method) => Instruction.Create(OpCodes.Call, method);

        /// <summary>
        /// A leave code.
        /// </summary>
        /// <param name="label">The leave instruction.</param>
        /// <returns></returns>
        public static Instruction Leave(Instruction label) => Instruction.Create(OpCodes.Leave, label);

        /// <summary>
        /// A stack load code.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<Instruction> Load(object value)
        {
            if (value == null)
            {
                yield return Null;
                yield break;
            }

            var type = value.GetType();

            if (type == typeof(string))
            {
                yield return String(value.ToString());
                yield break;
            }

            if (type == typeof(TypeReference) || type.IsSubclassOf(typeof(TypeReference)))
            {
                foreach (var instruction in TypeOf((TypeReference)value))
                    yield return instruction;

                yield break;
            }

            if (type.IsEnum)
                yield return Int((int)value);
            else if (type == typeof(bool))
                yield return Instruction.Create(OpCodes.Ldc_I4, (bool)value ? 1 : 0);
            else if (type == typeof(sbyte))
                yield return Instruction.Create(OpCodes.Ldc_I4, (sbyte)value);
            else if (type == typeof(short))
                yield return Instruction.Create(OpCodes.Ldc_I4, (short)value);
            else if (type == typeof(int))
                yield return Instruction.Create(OpCodes.Ldc_I4, (int)value);
            else if (type == typeof(long))
                yield return Instruction.Create(OpCodes.Ldc_I8, (long)value);
            else if (type == typeof(byte))
                yield return Instruction.Create(OpCodes.Ldc_I4, (byte)value);
            else if (type == typeof(ushort))
                yield return Instruction.Create(OpCodes.Ldc_I4, (ushort)value);
            else if (type == typeof(uint))
                yield return Instruction.Create(OpCodes.Ldc_I4, (uint)value);
            else if (type == typeof(ulong))
                yield return Instruction.Create(OpCodes.Ldc_I8, (ulong)value);
            else if (type == typeof(double))
                yield return Instruction.Create(OpCodes.Ldc_R8, (double)value);
            else if (type == typeof(float))
                yield return Instruction.Create(OpCodes.Ldc_R4, (float)value);

            if (type.IsEnum)
                yield return Box(type.IsEnum ? Enum.GetUnderlyingType(type).ToReference() : type.ToReference());

            yield break;
        }

        /// <summary>
        /// A stack load code.
        /// </summary>
        /// <param name="variable">The variable.</param>
        public static Instruction Load(Variable variable)
        {
            if (variable.IsField)
            {
                var field = (FieldReference)variable;
                return Instruction.Create(field.Resolve().IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field.Resolve().GetGeneric());
            }

            if (variable.IsLocal)
            {
                var local = (VariableDefinition)variable;
                
                switch (local.Index)
                {
                    case 0:
                        return Instruction.Create(OpCodes.Ldloc_0);
                    case 1:
                        return Instruction.Create(OpCodes.Ldloc_1);
                    case 2:
                        return Instruction.Create(OpCodes.Ldloc_2);
                    case 3:
                        return Instruction.Create(OpCodes.Ldloc_3);
                    default:
                        return Instruction.Create(OpCodes.Ldloc, (VariableDefinition)variable);
                }
            }

            if (variable.IsParameter)
                return Instruction.Create(OpCodes.Ldarg, (ParameterDefinition)variable);

            throw new NotSupportedException("Cannot load from an unrecognised variable");
        }

        /// <summary>
        /// A load token code.
        /// </summary>
        /// <param name="method">The method handle.</param>
        public static Instruction LoadToken(MethodReference method) => Instruction.Create(OpCodes.Ldtoken, method);

        /// <summary>
        /// A load token code.
        /// </summary>
        /// <param name="type">The type handle.</param>
        public static Instruction LoadToken(TypeReference type) => Instruction.Create(OpCodes.Ldtoken, type);

        /// <summary>
        /// A stack store code.
        /// </summary>
        /// <param name="variable">The variable.</param>
        public static Instruction Store(Variable variable)
        {
            if (variable.IsField)
            {
                var field = (FieldReference)variable;
                return Instruction.Create(field.Resolve().IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field.Resolve().GetGeneric());
            }

            if (variable.IsLocal)
            {
                var local = (VariableDefinition)variable;

                switch (local.Index)
                {
                    case 0:
                        return Instruction.Create(OpCodes.Stloc_0);
                    case 1:
                        return Instruction.Create(OpCodes.Stloc_1);
                    case 2:
                        return Instruction.Create(OpCodes.Stloc_2);
                    case 3:
                        return Instruction.Create(OpCodes.Stloc_3);
                    default:
                        return Instruction.Create(OpCodes.Stloc, (VariableDefinition)variable);
                }
            }

            if (variable.IsParameter)
                return Instruction.Create(OpCodes.Starg, ((ParameterReference)variable).Resolve());

            throw new NotSupportedException("Cannot store against an unrecognised variable");
        }

        /// <summary>
        /// A string load code.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Instruction String(string value) => Instruction.Create(OpCodes.Ldstr, value);

        /// <summary>
        /// A this stack load code only if the provided variable requires it.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns></returns>
        public static Instruction ThisIf(Variable variable) => variable.IsThisNeeded ? Instruction.Create(OpCodes.Ldarg_0) : (Instruction)null;

        /// <summary>
        /// A <c>typeof</c> code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<Instruction> TypeOf(TypeReference type)
        {
            yield return LoadToken(type);
            yield return InvokeStatic(ModuleWeaver.GlobalContext.Finder.TypeGetTypeFromHandle);
        }

        /// <summary>
        /// An unbox code.
        /// </summary>
        /// <param name="type">The type.</param>
        public static Instruction Unbox(TypeReference type) => Instruction.Create(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
    }

    /// <summary>
    /// A label class representing a position or offset within the method body.
    /// </summary>
    public class Label
    {
        private readonly List<Instruction> codes;

        private Instruction position;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        public Label()
        {
            codes = new List<Instruction>();
            position = null;
        }

        /// <summary>
        /// Reference the label against the provided code.
        /// </summary>
        /// <param name="code">The code.</param>
        public void Reference(Instruction code)
        {
            if (position != null)
                code.Operand = position;
            else
                codes.Add(code);
        }
        
        /// <summary>
        /// Mark the label at the provided code.
        /// </summary>
        /// <param name="code">The code.</param>
        public Instruction Mark(Instruction code)
        {
            if (position != null)
                throw new NotSupportedException($"Cannot mark a label more than once");

            codes.ForEach(c => c.Operand = code);
            codes.Clear();

            position = code;
            return code;
        }
    }

    /// <summary>
    /// A class representing a <c>try</c> block.
    /// </summary>
    public class TryBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TryBlock" /> class.
        /// </summary>
        public TryBlock()
        {

        }

        #region Properties

        /// <summary>
        /// Gets or sets the instruction where the <c>catch</c> block starts.
        /// </summary>
        public Instruction CatchStart { get; set; }

        /// <summary>
        /// Gets or sets the instruction where the <c>catch</c> block ends.
        /// </summary>
        public Instruction CatchEnd { get; set; }

        /// <summary>
        /// Gets or sets the instruction where the <c>finally</c> block starts.
        /// </summary>
        public Instruction FinallyStart { get; set; }

        /// <summary>
        /// Gets or sets the instruction where the <c>finally</c> block ends.
        /// </summary>
        public Instruction FinallyEnd { get; set; }

        /// <summary>
        /// Gets or sets the current block state.
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Gets or sets the instruction where the <c>try</c> block starts.
        /// </summary>
        public Instruction TryStart { get; set; }

        /// <summary>
        /// Gets or sets the instruction where the <c>try</c> block ends.
        /// </summary>
        public Instruction TryEnd { get; set; }

        #endregion
    }

    /// <summary>
    /// Indicates where code should be introduced when weaving.
    /// </summary>
    public enum CodeInsertion
    {
        /// <summary>
        /// The code should be appended to the end of the method body.
        /// </summary>
        Append,

        /// <summary>
        /// The code should be inserted after the current instruction.
        /// </summary>
        After,

        /// <summary>
        /// The code should be inserted before the current instruction.
        /// </summary>
        Before,
    }
}
