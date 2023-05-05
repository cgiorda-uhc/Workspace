


namespace VCPortal_WebUI.Client.Utilities;
public static class JSUtilities
{
    public async static Task SaveAs(IJSRuntime js, string filename, byte[] data)
    {
        await js.InvokeAsync<object>(
            "saveAsFile",
            filename,
            Convert.ToBase64String(data));
    }
}
