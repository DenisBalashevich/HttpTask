using System;

namespace HttpTask
{
    public  interface ILimitation
    {
        bool HasLimitation(Uri uri);
    }
}
