using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Extensions;
using Thinktecture.IdentityServer.Core.Services;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class ConsentService : IConsentService
    {
        private readonly CoreDbContextFactoryBase _dbFactory;

        public ConsentService(CoreDbContextFactoryBase dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public Task<bool> RequiresConsentAsync(Models.Client client, System.Security.Claims.ClaimsPrincipal user, IEnumerable<string> scopes)
        {
            if (!client.RequireConsent)
            {
                return Task.FromResult(false);
            }

            var orderedScopes = string.Join(" ", scopes.OrderBy(s => s).ToArray());

            string subjectId = user.GetSubjectId();

            using (var db = _dbFactory.Create())
            {
                var exists = db.Consents.Any(c => c.ClientId == client.ClientId &&
                                                c.Scopes == orderedScopes &&
                                                c.Subject == subjectId);

                return Task.FromResult(!exists);
            }
        }

        public async Task UpdateConsentAsync(Models.Client client, System.Security.Claims.ClaimsPrincipal user, IEnumerable<string> scopes)
        {
            if (client.AllowRememberConsent)
            {
                using (var db = _dbFactory.Create())
                {
                    var clientId = client.ClientId;
                    var subject = user.GetSubjectId();

                    var consent = await db.Consents.FindAsync(subject, clientId);

                    if (scopes.Any())
                    {
                        if (consent == null)
                        {
                            consent = new Entities.Consent
                            {
                                ClientId = client.ClientId,
                                Subject = user.GetSubjectId(),
                            };
                            db.Consents.Add(consent);
                        }

                        consent.Scopes = string.Join(" ", scopes.OrderBy(s => s).ToArray());
                    }
                    else if (consent != null)
                    {
                        db.Consents.Remove(consent);
                    }

                    db.SaveChanges();
                }
            }
        }
    }
}
