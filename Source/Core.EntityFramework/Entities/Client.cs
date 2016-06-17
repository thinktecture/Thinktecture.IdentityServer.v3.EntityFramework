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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IdentityServer3.Core.Models;

namespace IdentityServer3.EntityFramework.Entities
{
    public class Client
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual bool Enabled { get; set; }

        [Required]
        [StringLength(200)]
        [Index(IsUnique=true)]
        public virtual string ClientId { get; set; }

        public virtual ICollection<ClientSecret> ClientSecrets { get; set; }
        
        [Required]
        [StringLength(200)]
        public virtual string ClientName { get; set; }
        [StringLength(2000)]
        public virtual string ClientUri { get; set; }
        public virtual string LogoUri { get; set; }

        public virtual bool RequireConsent { get; set; }
        public virtual bool AllowRememberConsent { get; set; }
        public virtual bool AllowAccessTokensViaBrowser { get; set; }

        public virtual Flows Flow { get; set; }
        public virtual bool AllowClientCredentialsOnly { get; set; }

        public virtual ICollection<ClientRedirectUri> RedirectUris { get; set; }
        public virtual ICollection<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }

        public virtual string LogoutUri { get; set; }
        public virtual bool LogoutSessionRequired { get; set; }
        public virtual bool RequireSignOutPrompt { get; set; }

        public virtual bool AllowAccessToAllScopes { get; set; }
        public virtual ICollection<ClientScope> AllowedScopes { get; set; }

        // in seconds
        [Range(0, Int32.MaxValue)]
        public virtual int IdentityTokenLifetime { get; set; }
        [Range(0, Int32.MaxValue)]
        public virtual int AccessTokenLifetime { get; set; }
        [Range(0, Int32.MaxValue)]
        public virtual int AuthorizationCodeLifetime { get; set; }
        
        [Range(0, Int32.MaxValue)]
        public virtual int AbsoluteRefreshTokenLifetime { get; set; }
        [Range(0, Int32.MaxValue)]
        public virtual int SlidingRefreshTokenLifetime { get; set; }

        public virtual TokenUsage RefreshTokenUsage { get; set; }
        public virtual bool UpdateAccessTokenOnRefresh { get; set; }

        public virtual TokenExpiration RefreshTokenExpiration { get; set; }
        
        public virtual AccessTokenType AccessTokenType { get; set; }

        public virtual bool EnableLocalLogin { get; set; }
        public virtual ICollection<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        
        public virtual bool IncludeJwtId { get; set; }

        public virtual ICollection<ClientClaim> Claims { get; set; }
        public virtual bool AlwaysSendClientClaims { get; set; }
        public virtual bool PrefixClientClaims { get; set; }

        public virtual bool AllowAccessToAllGrantTypes { get; set; }

        public virtual ICollection<ClientCustomGrantType> AllowedCustomGrantTypes { get; set; }
        public virtual ICollection<ClientCorsOrigin> AllowedCorsOrigins { get; set; }
    }
}
