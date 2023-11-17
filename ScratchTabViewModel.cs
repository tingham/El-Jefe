using Microsoft.VisualStudio.PlatformUI;

namespace El_Jefe
{
  class ScratchTabViewModel : ObservableObject
  {
    public string Title { get; internal set; }

    public ScratchTabViewModel() {
      Title = "Base Scratch Title";
    }
  }
}