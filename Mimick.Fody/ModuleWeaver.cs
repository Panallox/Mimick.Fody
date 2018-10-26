using Fody;
using Mimick.Fody;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The foundation weaver definition containing the core execution instructions.
/// </summary>
public partial class ModuleWeaver : BaseModuleWeaver
{
    #region Properties

    /// <summary>
    /// Gets or sets the action used to log information.
    /// </summary>
    public static Action<string> Log { get; set; }

    /// <summary>
    /// Gets or sets the context used when weaving.
    /// </summary>
    public WeaveContext Context { get; set; }

    #endregion

    /// <summary>
    /// Called when the weaver is executed.
    /// </summary>
    public override void Execute()
    {
        Log = LogInfo;

        Context = new WeaveContext(ModuleDefinition);
        Context.Candidates = new WeaveCandidates(ModuleDefinition);
        Context.Refs = new WeaveReferences(ModuleDefinition);
        
        WeaveInterceptors();
    }

    /// <summary>
    /// Return a list of assembly names for scanning.
    /// Used as a list for <see cref="P:Fody.BaseModuleWeaver.FindType" />.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
        yield return "System.Runtime";
    }
}