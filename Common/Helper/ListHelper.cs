using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class ListHelper
    {
        public static bool Any<T>(ICollection<T> list)
        {
            return (list != null && list.Any());
        }
    }
}
