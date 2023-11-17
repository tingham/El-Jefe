using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Runtime.InteropServices;

namespace El_Jefe
{
  /// <summary>
  /// This class implements the tool window exposed by this package and hosts a user control.
  /// </summary>
  /// <remarks>
  /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
  /// usually implemented by the package implementer.
  /// <para>
  /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
  /// implementation of the IVsUIElementPane interface.
  /// </para>
  /// </remarks>
  [Guid("fcf44d45-a1af-4606-a0e3-bd8f57d0b97b")]
  public class AsyncPlainView : ToolWindowPane
  {
    private AsyncPlainViewCommand elJefeCommand;
    internal AsyncPlainViewCommand ElJefeCommand { 
      get => this.elJefeCommand;
      // Write-once property
      set => this.elJefeCommand = value;
    }

    public SettingsStore ExtensionSettings
    {
      get
      {
        var settings = Properties.Settings.Default;

        SettingsManager settingsManager = new ShellSettingsManager(this);
        SettingsStore store = settingsManager.GetWritableSettingsStore(SettingsScope.Configuration);
        return store;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPlainView"/> class.
    /// </summary>
    public AsyncPlainView() : base(null)
    {
      this.Caption = "El Jefe: Vanilla View";

      // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
      // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
      // the object returned by the Content property.
      var control = new AsyncPlainViewControl(this);
      control.HostView = this;
      this.Content = control;
    }
  }
}
