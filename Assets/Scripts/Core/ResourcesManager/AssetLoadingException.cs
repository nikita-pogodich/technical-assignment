using System;

namespace Core.ResourcesManager
{
    public class AssetLoadingException : Exception
    {
        public AssetLoadingException(string message) : base(message)
        {
        }
    }
}