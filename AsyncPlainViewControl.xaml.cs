using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using System.Drawing;
using MessagePack.Formatters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace El_Jefe
{
  /// <summary>
  /// Interaction logic for AsyncPlainViewControl.
  /// </summary>
  public partial class AsyncPlainViewControl : UserControl
  {
    public AsyncPlainView HostView { get; set; }

    public string SearchText { get; set; }

    private Range viewerRange = new Range(0, 0);
    public Range ViewerRange { get => viewerRange; set => viewerRange = value; }

    // Observable properties for FontSetting:
    // _Family
    // _Size
    // _Color
    
    public FontFamily FontSetting_Family;
    public double FontSetting_Size;
    public System.Drawing.Color FontSetting_Color;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncPlainViewControl"/> class.
    /// </summary>
    public AsyncPlainViewControl(AsyncPlainView hostView)
    {
      this.InitializeComponent();

      foreach (TabItem tabItem in DocumentTabPanel.Items)
      {
        tabItem.Visibility = Visibility.Collapsed;
      }

      this.HostView = hostView;

      string scratchValue = Properties.Settings.Default.Scratch;
      string previousDirectoryValue = Properties.Settings.Default.PreviousDirectory;

      System.Diagnostics.Debug.WriteLine($"{scratchValue} {previousDirectoryValue}");
    }

    /// <summary>
    /// Handles click on the button by displaying a message box.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
    private void button1_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show(
          string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
          "AsyncPlainView");
    }

    private void ToolBarTray_Loaded(object sender, RoutedEventArgs e)
    {
      ToolBarTray tray = sender as ToolBarTray;
      // Get all toolbars in the tray
      var toolBars = tray.ToolBars;
      foreach (var bar in toolBars)
      {
        if (bar.Template.FindName("OverflowGrid", bar) is FrameworkElement overflowElement)
        {
          overflowElement.Visibility = Visibility.Hidden;
        }
        if (bar.Template.FindName("MainPanelBorder", bar) is FrameworkElement panelBorder)
        {
          panelBorder.Margin = new Thickness(0);
        }
      }
    }

    private void MenuItem_Selected(object sender, RoutedEventArgs e)
    {
      if (sender is MenuItem menuItem)
      {
        string currentHeader = menuItem.Header.ToString();
        if (currentHeader == SwitchToTextMenuItem.Header.ToString())
        {
          Plain.IsSelected = true;
        }
        else if (currentHeader == SwitchToTableMenuItem.Header.ToString())
        {
          Tab.IsSelected = true;
        }
        else if (currentHeader == Scratch.Header.ToString())
        {
          var scratchTabClone = ScratchTemplate.CacheMode.Clone();
          scratchTabClone.
        }
      }
    }

    private void NewScratchTab(string fromData = null) {
      if (fromData != null)
      {
        fromData = "";
      }
      var scratchPanel = new ScratchTabPanel();
      // Format a date string for the title of the panel's contents (e.g. "Scratch 2020-01-01 12:00:00")
      scratchPanel.Title.Content = $"Scratch: {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssz")}";
      // Add the panel to the DocumentTabPanel
      DocumentTabPanel.Items.Add(scratchPanel);
      DocumentTabPanel.SelectedItem = scratchPanel;
      // Add a new entry in Properties.Settings.Default.Scratch json
      string scratchValue = Properties.Settings.Default.Scratch;
      JObject scratch;

      try
      {
        scratch = JObject.Parse(scratchValue);
      }
      catch {
        scratch = new JObject();
      }

      // Add a new entry in scratch with the title as the key and an empty string as the value
      scratch.Add(scratchPanel.Title.Content.ToString(), "");

      // Bind the panel's contents so that we can observe changes to the text and save them to the json
      scratchPanel.ScratchTextBox.TextChanged += (s, e) => {
        // Save the text to the json
        if (s is TextBox textBox)
        {
          System.Diagnostics.Debug.Write($"Wrote something to {scratchPanel.Title.Content}");
          scratch.Add(scratchPanel.Title.Content.ToString(), textBox.Text.ToString());
        }
      };
    }

    private void Search_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      // Cancel the current time delay for triggering a refinement of the data

      // Schedule a delay to trigger a refinement of the data and store the timer

    }

    private void LoadTextBlock_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {

    }

    private void DocumentTabPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void OpenDirectory_Click(object sender, RoutedEventArgs e)
    {
      // Open a file dialog to select a directory (or file)
      FileDialog fileDialog = new OpenFileDialog();
      fileDialog.Filter = "All Files (*.*)|*.*";
      var result = fileDialog.ShowDialog();

      if (result.HasValue && result == true)
      {
        // Get the directory path from the first file selected
        string directoryPath = Path.GetDirectoryName(fileDialog.FileName);
        this.FilesAsMemoryMappedFileUnion(directoryPath, fileDialog.FileNames.ToList());
      }
    }

    private MemoryMappedFile FilesAsMemoryMappedFileUnion(string directoryPath, List<string> paths)
    {
      // We are going to prepare a single aggregate file that contains all the files in the directory without loading them into memory
      // Once we have a temporary file on disk, we will create a MemoryMappedFile object to access the file as a stream
      // We will then use the MemoryMappedFile object to create a StreamReader object to read the file line by line based on this.ViewerRange

      if (paths.Count == 0) return null;

      if (paths.Count == 1)
      {
        using (FileStream readStream = new FileStream(paths.First(), FileMode.Open, FileAccess.Read))
        {
          int capacity = (int)readStream.Length;
          MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(paths.First(), FileMode.Open, paths.First(), capacity);
          return memoryMappedFile;
        }
      }
      else
      {
        byte[] buffer = new byte[0];
        int offset = 0;
        int length = 0;
        Dictionary<string, Tuple<FileStream, int>> streams = new Dictionary<string, Tuple<FileStream, int>>();

        foreach (var path in paths)
        {
          using (FileStream readStream = new FileStream(path, FileMode.Open, FileAccess.Read))
          {
            offset = (int)readStream.Length;
            Tuple<FileStream, int> entry = new Tuple<FileStream, int>(readStream, offset);
            streams.Add(path, entry);
          }
          length += offset;
        }

        foreach (var streamRecord in streams)
        {
          var path = streamRecord.Key;
          var stream = streamRecord.Value.Item1 as Stream;
          var currentOffset = streamRecord.Value.Item2;

          using (var mappedFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, length))
          {
            using (var reader = mappedFile.CreateViewStream(0, length, MemoryMappedFileAccess.Read))
            {
              // Read from MMF
              buffer = new byte[length];
              reader.Read(buffer, 0, length);
            }
          }

        }

        using (var outputMappedFile = MemoryMappedFile.CreateFromFile(GenerateTempFilePath(directoryPath), FileMode.Open, null, offset + length))
        {
          // Create writer to MMF
          using (var writer = outputMappedFile.CreateViewAccessor(offset, length, MemoryMappedFileAccess.Write))
          {
            // Write to MMF
            writer.WriteArray<byte>(0, buffer, 0, length);
          }
          return outputMappedFile;
        }
      }
    }

    private string GenerateTempFilePath(string originalPath)
    {
      string guid = Guid.NewGuid().ToString();
      return $"{originalPath}-{guid}";
    }

  }

  public struct Range
{
  [ReadOnly(true)]
  public int Start;
  [ReadOnly(true)]
  public int End;
  public int Length => End - Start;

  public Range(int start, int end)
  {
    Start = start;
    End = end;
  }

  // Add implicit conversion from Range to System.Range
}
}