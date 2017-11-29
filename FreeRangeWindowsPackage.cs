using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Shmuelie.FreeRangeWindows
{
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(FreeRangeWindowsPackage.PackageGuidString)]
    [CLSCompliant(false)]
    public sealed class FreeRangeWindowsPackage : Package
    {
        /// <summary>
        /// FreeRangeWindowsPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "214337a7-2957-45e5-b42e-b8cf91cbe11f";

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.VisualStudio.Shell.ActivityLog.LogInformation(System.String,System.String)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.VisualStudio.Shell.ActivityLog.TryLogWarning(System.String,System.String)")]
        protected override void Initialize()
        {
            base.Initialize();
            Assembly viewManagerAssembly = Assembly.Load("Microsoft.VisualStudio.Shell.ViewManager");
            if (viewManagerAssembly == null)
            {
                ActivityLog.TryLogWarning(nameof(FreeRangeWindows), "Unable to load ViewManager");
                return;
            }
            Type floatingWindowType = viewManagerAssembly.GetType("Microsoft.VisualStudio.PlatformUI.Shell.Controls.FloatingWindow", false);
            if (floatingWindowType == null)
            {
                ActivityLog.TryLogWarning(nameof(FreeRangeWindows), "Unable to find FloatingWindow");
                return;
            }
            ActivityLog.LogInformation(nameof(FreeRangeWindows), "Registering for FloatingWindow changes");
            UIElement.VisibilityProperty.OverrideMetadata(floatingWindowType, new FrameworkPropertyMetadata(Visibility.Collapsed, FloatingWindowVisibilityChanged));
        }

        private readonly static Lazy<Func<Window, bool>> lazyChangeOwner = new Lazy<Func<Window, bool>>(CreateChangeOwner, true);

        private static bool Empty(Window window) => false;

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.VisualStudio.Shell.ActivityLog.LogInformation(System.String,System.String)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.VisualStudio.Shell.ActivityLog.TryLogWarning(System.String,System.String)")]
        private static Func<Window, bool> CreateChangeOwner()
        {
            Type floatingWindowType = Assembly.Load("Microsoft.VisualStudio.Shell.ViewManager").GetType("Microsoft.VisualStudio.PlatformUI.Shell.Controls.FloatingWindow", false);
            MethodInfo changeOwnerInfo = floatingWindowType.GetMethod("ChangeOwner", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public, null, new[] { typeof(IntPtr) }, null);
            if (changeOwnerInfo == null)
            {
                ActivityLog.TryLogWarning(nameof(FreeRangeWindows), "Unable to find ChangeOwner");
                return Empty;
            }
            DynamicMethod dynamicChangeOwner = new DynamicMethod(changeOwnerInfo.Name, typeof(bool), new[] { typeof(Window) }, true);
            ILGenerator generator = dynamicChangeOwner.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, floatingWindowType);
            generator.Emit(OpCodes.Ldsfld, typeof(IntPtr).GetField(nameof(IntPtr.Zero)));
            generator.Emit(OpCodes.Call, changeOwnerInfo);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Ret);
            ActivityLog.LogInformation(nameof(FreeRangeWindows), "Created ChangeOwner wrapper");
            return (Func<Window, bool>)dynamicChangeOwner.CreateDelegate(typeof(Func<Window, bool>));
        }

#pragma warning disable U2U1003 // Avoid declaring methods used in delegate constructors static
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.VisualStudio.Shell.ActivityLog.LogInformation(System.String,System.String)")]
        private static void FloatingWindowVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
#pragma warning restore U2U1003 // Avoid declaring methods used in delegate constructors static
        {
            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                Window window = (Window)d;
                if (lazyChangeOwner.Value(window))
                {
                    ActivityLog.LogInformation(nameof(FreeRangeWindows), "Making " + window.Title + " free range");
                    window.ShowInTaskbar = true;
                }
            }
        }
    }
}
