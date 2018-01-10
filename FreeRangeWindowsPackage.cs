using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualStudio.Platform.WindowManagement;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
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

        protected override void Initialize()
        {
            base.Initialize();
            ActivityLog.LogInformation(nameof(FreeRangeWindows), "Registering for FloatingWindow changes");
            UIElement.VisibilityProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(Visibility.Collapsed, FloatingWindowVisibilityChanged));
            ViewElement.IsSelectedProperty.OverrideMetadata(typeof(View), new PropertyMetadata(false, ViewElementBooleanDependencyPropertyChanged));
            ViewElement.IsSelectedProperty.OverrideMetadata(typeof(ToolWindowView), new PropertyMetadata(false, ViewElementBooleanDependencyPropertyChanged));
            View.IsActiveProperty.OverrideMetadata(typeof(ToolWindowView), new FrameworkPropertyMetadata(false, ViewElementBooleanDependencyPropertyChanged));
        }

#pragma warning disable U2U1003 // Avoid declaring methods used in delegate constructors static
        private static void ViewElementBooleanDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
#pragma warning restore U2U1003 // Avoid declaring methods used in delegate constructors static
        {
            if ((bool)e.NewValue && d != null && d is View view && view.Content != null)
            {
                UIElement content = (UIElement)view.Content;
                Window window = Window.GetWindow(content);
                if (window != null)
                {
                    BindTitle(window, view);
                }
                else
                {
                    void isVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
                    {
                        UIElement element = (UIElement)sender;
                        if ((bool)args.NewValue)
                        {
                            BindTitle(Window.GetWindow(element), view);
                        }
                        element.IsVisibleChanged -= isVisibleChanged;
                    }
                    content.IsVisibleChanged += isVisibleChanged;
                }
            }
        }

        private static void BindTitle(Window window, View view)
        {
            string mainTitle = Application.Current.MainWindow.Title;
            window.SetBinding(Window.TitleProperty, new Binding("Title")
            {
                FallbackValue = mainTitle,
                Mode = BindingMode.OneWay,
                StringFormat = mainTitle + " - {0}",
                Source = view
            });
        }

#pragma warning disable U2U1003 // Avoid declaring methods used in delegate constructors static
        private static void FloatingWindowVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
#pragma warning restore U2U1003 // Avoid declaring methods used in delegate constructors static
        {
            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                FloatingWindow window = (FloatingWindow)d;
                window.ChangeOwner(IntPtr.Zero);
                window.ShowInTaskbar = true;
                ActivityLog.LogInformation(nameof(FreeRangeWindows), "Making " + window.Title + " free range");
            }
        }
    }
}
