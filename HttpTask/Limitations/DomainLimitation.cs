using System;

namespace HttpTask
{
    public class DomainLimitation: ILimitation
    {
        private readonly Uri _parentUri;
        private readonly DomainLimitationType _availableTransition;

        public DomainLimitation(DomainLimitationType availableTransition, Uri parentUri)
        {
            switch (availableTransition)
            {
                case DomainLimitationType.All:
                case DomainLimitationType.Current:
                case DomainLimitationType.NotHigherThanInUrl:
                    _availableTransition = availableTransition;
                    _parentUri = parentUri;
                    break;
                default:
                    throw new ArgumentException($"Unknown transition type: {availableTransition}");
            }
        }

        public bool HasLimitation(Uri uri)
        {
            switch (_availableTransition)
            {
                case DomainLimitationType.All:
                    return true;
                case DomainLimitationType.Current:
                    if (_parentUri.DnsSafeHost == uri.DnsSafeHost)
                    {
                        return true;
                    }
                    break;
                case DomainLimitationType.NotHigherThanInUrl:
                    if (_parentUri.IsBaseOf(uri))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
