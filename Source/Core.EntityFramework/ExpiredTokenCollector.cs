using System;
using System.Linq;
using System.Timers;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ExpiredTokenCollector
    {
        private static readonly Timer Timer = new Timer();
        private static CoreDbContextFactoryBase _dbFactory;

        public static void Start(CoreDbContextFactoryBase dbFactory, int cleanupIntervalInMinutes)
        {
            _dbFactory = dbFactory;

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

            using (var db =_dbFactory.Create())
            {
                db.Tokens.RemoveRange(db.Tokens.Where(c => c.Expiry < referenceDate));
                db.SaveChanges();
            }
        }
    }
}
