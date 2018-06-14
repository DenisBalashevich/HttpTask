using System;
using System.Collections.Generic;
using System.Linq;

namespace HttpTask
{
    public class FileFormatLimitation : ILimitation
    {
        private readonly IEnumerable<string> _acceptableExtensions;

        public FileFormatLimitation(IEnumerable<string> acceptableExtensions)
        {
            _acceptableExtensions = acceptableExtensions;
        }

        public bool HasLimitation(Uri uri)
        {
            var lastSegment = uri.Segments.Last();
            return _acceptableExtensions.Any(e => lastSegment.EndsWith(e));
        }
    }
}
