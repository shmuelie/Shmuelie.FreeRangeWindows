using System.Resources;
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\Nerdbank.Streams.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\Newtonsoft.Json.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\Shmuelie.FreeRangeWindows.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\StreamJsonRpc.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\System.Buffers.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\System.IO.Pipelines.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\System.Memory.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\System.Numerics.Vectors.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\System.Runtime.CompilerServices.Unsafe.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\System.Threading.Tasks.Extensions.dll")]
[assembly: ProvideCodeBase(CodeBase = @"$PackageFolder$\System.ValueTuple.dll")]

