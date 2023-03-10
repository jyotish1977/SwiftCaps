namespace SwiftCaps.Client.Core.Interfaces
{
    /// <summary>
    /// Open up a URL in the Device Native browser
    /// </summary>
    public interface INativeBrowserService
    {
        void LaunchNativeBrowser(string url);
    }
}
