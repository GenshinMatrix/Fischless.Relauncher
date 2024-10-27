using System.IO;
using System.IO.Packaging;
using System.Windows.Resources;

namespace Fischless.Relauncher.Extensions;

internal static class ResourceHelper
{
    static ResourceHelper()
    {
        if (!UriParser.IsKnownScheme("pack"))
            _ = PackUriHelper.UriSchemePack;
    }

    public static Stream GetStream(string uriString)
    {
        Uri uri = new(uriString);
        StreamResourceInfo info = Application.GetResourceStream(uri);
        return info?.Stream!;
    }
}
