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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure.Annotations;
using IdentityServer3.EntityFramework.Entities;

namespace IdentityServer3.EntityFramework
{
    public static class DbModelBuilderExtensions
    {
        public static void RegisterClientChildTablesForDelete<TClient>(this DbContext ctx) 
            where TClient : Client 
        {
            ctx.Set<TClient>().Local.CollectionChanged +=
                delegate (object sender, NotifyCollectionChangedEventArgs e)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (Client item in e.NewItems)
                        {
                            RegisterDeleteOnRemove(item.ClientSecrets, ctx);
                            RegisterDeleteOnRemove(item.RedirectUris, ctx);
                            RegisterDeleteOnRemove(item.PostLogoutRedirectUris, ctx);
                            RegisterDeleteOnRemove(item.AllowedScopes, ctx);
                            RegisterDeleteOnRemove(item.IdentityProviderRestrictions, ctx);
                            RegisterDeleteOnRemove(item.Claims, ctx);
                            RegisterDeleteOnRemove(item.AllowedCustomGrantTypes, ctx);
                            RegisterDeleteOnRemove(item.AllowedCorsOrigins, ctx);
                        }
                    }
                };
        }

        public static void RegisterScopeChildTablesForDelete<TScope>(this DbContext ctx)
            where TScope : Scope
        {
            ctx.Set<TScope>().Local.CollectionChanged +=
                delegate (object sender, NotifyCollectionChangedEventArgs e)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (Scope item in e.NewItems)
                        {
                            RegisterDeleteOnRemove(item.ScopeClaims, ctx);
                            RegisterDeleteOnRemove(item.ScopeSecrets, ctx);
                        }
                    }
                };
        }

        public static void RegisterConsentChildTablesForDelete<TConsent>(this DbContext ctx)
            where TConsent : Consent
        {
            // empty
        }

        public static void RegisterTokenChildTablesForDelete<TToken>(this DbContext ctx)
            where TToken : Token
        {
            // empty
        }

        internal static void RegisterDeleteOnRemove<TChild>(this ICollection<TChild> collection, DbContext ctx)
            where TChild : class
        {
            var entities = collection as EntityCollection<TChild>;
            if (entities != null)
            {
                entities.AssociationChanged += delegate (object sender, CollectionChangeEventArgs e)
                {
                    if (e.Action == CollectionChangeAction.Remove)
                    {
                        var entity = e.Element as TChild;
                        if (entity != null)
                        {
                            ctx.Entry(entity).State = EntityState.Deleted;
                        }
                    }
                };
            }
        }

        public static void ConfigureClients(this DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Entity<Client>()
                .ToTable(EfConstants.TableNames.Client, schema);
            modelBuilder.Entity<Client>()
                .HasMany(x => x.ClientSecrets).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.RedirectUris).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.PostLogoutRedirectUris).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.AllowedScopes).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.IdentityProviderRestrictions).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.Claims).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.AllowedCustomGrantTypes).WithRequired(x => x.Client).WillCascadeOnDelete();
            modelBuilder.Entity<Client>()
                .HasMany(x => x.AllowedCorsOrigins).WithRequired(x => x.Client).WillCascadeOnDelete();

            modelBuilder.Entity<ClientSecret>().ToTable(EfConstants.TableNames.ClientSecret, schema);
            modelBuilder.Entity<ClientRedirectUri>().ToTable(EfConstants.TableNames.ClientRedirectUri, schema);
            modelBuilder.Entity<ClientPostLogoutRedirectUri>().ToTable(EfConstants.TableNames.ClientPostLogoutRedirectUri, schema);
            modelBuilder.Entity<ClientScope>().ToTable(EfConstants.TableNames.ClientScopes, schema);
            modelBuilder.Entity<ClientIdPRestriction>().ToTable(EfConstants.TableNames.ClientIdPRestriction, schema);
            modelBuilder.Entity<ClientClaim>().ToTable(EfConstants.TableNames.ClientClaim, schema);
            modelBuilder.Entity<ClientCustomGrantType>().ToTable(EfConstants.TableNames.ClientCustomGrantType, schema);
            modelBuilder.Entity<ClientCorsOrigin>().ToTable(EfConstants.TableNames.ClientCorsOrigin, schema);
        }

        public static void ConfigureConsents(this DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Entity<Consent>().ToTable(EfConstants.TableNames.Consent, schema);
        }

        public static void ConfigureTokens(this DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Entity<Token>().ToTable(EfConstants.TableNames.Token, schema);
            modelBuilder.Entity<Token>().Property(x => x.Expiry)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("Expiry") { IsUnique = false}));
        }
        public static void ConfigureScopes(this DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Entity<Scope>().ToTable(EfConstants.TableNames.Scope, schema);

            modelBuilder.Entity<Scope>()
                .HasMany(x => x.ScopeClaims).WithRequired(x => x.Scope).WillCascadeOnDelete();
            modelBuilder.Entity<Scope>()
                .HasMany(x => x.ScopeSecrets).WithRequired(x => x.Scope).WillCascadeOnDelete();

            modelBuilder.Entity<ScopeClaim>().ToTable(EfConstants.TableNames.ScopeClaim, schema);
            modelBuilder.Entity<ScopeSecret>().ToTable(EfConstants.TableNames.ScopeSecrets, schema);
        }
    }
}