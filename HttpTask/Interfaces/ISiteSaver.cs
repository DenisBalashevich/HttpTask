using System;
using System.IO;

namespace HttpTask
{
    public interface ISiteSaver
    {
        void SaveHtmlDocument(Uri uri, string name, Stream documentStream);
    }
}
