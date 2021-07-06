//CODE TAKEN FROM https://stackoverflow.com/questions/47874173/dictionary-cache-with-expiration-time
//CREDITS TO Hannen, Scott, 2017.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic
{
    public class CacheItem<T>
    {
        public CacheItem(T value, TimeSpan expiresAfter)
        {
            Value = value;
            ExpiresAfter = expiresAfter;
        }
        public T Value { get; }
        
        public DateTimeOffset Created = DateTimeOffset.Now;
        internal TimeSpan ExpiresAfter { get; }
    }
}
