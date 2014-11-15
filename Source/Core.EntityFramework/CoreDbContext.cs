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
using System.Data.Entity;
using Thinktecture.IdentityServer.Core.EntityFramework.Entities;

namespace Thinktecture.IdentityServer.Core.EntityFramework
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(string connectionString)
            : base(connectionString)
        {
            Configuration.ProxyCreationEnabled = false;
        }

        public CoreDbContext()
            : this("Thinktecture.IdentityServer.Core")
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<Consent> Consents { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasMany(x => x.RedirectUris).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.PostLogoutRedirectUris).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.ScopeRestrictions).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.IdentityProviderRestrictions).WithRequired(x => x.Client).WillCascadeOnDelete();
            
            modelBuilder.Entity<Scope>()
                .HasMany(x => x.ScopeClaims).WithRequired(x => x.Scope).WillCascadeOnDelete();
        }
    }
}
