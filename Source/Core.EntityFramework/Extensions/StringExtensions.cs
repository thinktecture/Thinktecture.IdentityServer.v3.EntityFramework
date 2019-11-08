/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdentityServer3.EntityFramework
{
    internal static class StringExtensions
    {
        public static string GetOrigin(this string url)
        {
            if (url != null && (url.StartsWith("http://") || url.StartsWith("https://")))
            {
                var idx = url.IndexOf("//");
                if (idx > 0)
                {
                    idx = url.IndexOf("/", idx + 2);
                    if (idx >= 0)
                    {
                        url = url.Substring(0, idx);
                    }
                    return url;
                }
            }

            return null;
        }

        public static string GetAsCommaSeparatedString(this IEnumerable<string> collection)
        {
            if (collection == null || !collection.Any())
            {
                return null;
            }

            var builder = new StringBuilder();

            foreach (var item in collection)
            {
                builder.Append(item);
                builder.Append(",");
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}
