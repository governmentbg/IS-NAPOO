namespace ISNAPOO.Core.HelperClasses
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.JSInterop;

    public static class FileUtils
    {
        public static async Task<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
           => await js.InvokeAsync<object>(
               "saveAsFile",
               filename,
               Convert.ToBase64String(data));

        public static void SaveAs(string v, byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
