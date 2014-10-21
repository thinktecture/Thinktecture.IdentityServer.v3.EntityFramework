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
using System;
using System.Linq;
using System.Timers;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ExpiredTokenCollector
    {
        private static CoreDbContext _db;
        private static readonly Timer Timer = new Timer();

        public static void Start(CoreDbContext db, int cleanupIntervalInMinutes)
        {
            _db = db;

            Timer.AutoReset = true;
            Timer.Interval = cleanupIntervalInMinutes * 60 * 1000;

            Timer.Elapsed += CleanUpTokens;
            Timer.Start();
        }

        public static void Stop()
        {
            Timer.Stop();
        }

        private static void CleanUpTokens(object sender, ElapsedEventArgs e)
        {
            // Clean up expired tokens
            DateTime referenceDate = DateTime.UtcNow;

            var db = _db;
            {
                db.Tokens.RemoveRange(db.Tokens.Where(c => c.Expiry < referenceDate));
                db.SaveChanges();
            }
        }
    }
}
