using AutoMapper;
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
using System.Security.Claims;
using Entities = Thinktecture.IdentityServer.EntityFramework.Entities;

namespace Thinktecture.IdentityServer.Core.Models
{
    public static class EntitiesMap
    {
        static EntitiesMap()
        {
            Mapper.CreateMap<Models.Scope, Entities.Scope>(MemberList.Source)
                .ForSourceMember(x => x.Claims, opts => opts.Ignore())
                .ForMember(x => x.ScopeClaims, opts => opts.MapFrom(src => src.Claims.Select(x => x)));
            Mapper.CreateMap<Models.ScopeClaim, Entities.ScopeClaim>(MemberList.Source);

            Mapper.CreateMap<Models.ClientSecret, Entities.ClientSecret>(MemberList.Source);
            Mapper.CreateMap<Models.Client, Entities.Client>(MemberList.Source)
                .ForMember(x => x.CustomGrantTypeRestrictions, opt => opt.MapFrom(src => src.CustomGrantTypeRestrictions.Select(x => new Entities.ClientGrantTypeRestriction { GrantType = x })))
                .ForMember(x => x.RedirectUris, opt => opt.MapFrom(src => src.RedirectUris.Select(x => new Entities.ClientRedirectUri { Uri = x })))
                .ForMember(x => x.PostLogoutRedirectUris, opt => opt.MapFrom(src => src.PostLogoutRedirectUris.Select(x => new Entities.ClientPostLogoutRedirectUri { Uri = x })))
                .ForMember(x => x.IdentityProviderRestrictions, opt => opt.MapFrom(src => src.IdentityProviderRestrictions.Select(x => new Entities.ClientIdPRestriction { Provider = x })))
                .ForMember(x => x.ScopeRestrictions, opt => opt.MapFrom(src => src.ScopeRestrictions.Select(x => new Entities.ClientScopeRestriction { Scope = x })))
                .ForMember(x => x.AllowedCorsOrigins, opt => opt.MapFrom(src => src.AllowedCorsOrigins.Select(x => new Entities.ClientCorsOrigin { Origin = x })))
                .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => new Entities.ClientClaim { Type = x.Type, Value = x.Value })));
        }

        public static Entities.Scope ToEntity(this Models.Scope s)
        {
            if (s == null) return null;

            if (s.Claims == null)
            {
                s.Claims = new List<Models.ScopeClaim>();
            }

            return Mapper.Map<Models.Scope, Entities.Scope>(s);
        }

        public static Entities.Client ToEntity(this Models.Client s)
        {
            if (s == null) return null;

            if (s.ClientSecrets == null)
            {
                s.ClientSecrets = new List<ClientSecret>();
            }
            if (s.RedirectUris == null)
            {
                s.RedirectUris = new List<string>();
            }
            if (s.PostLogoutRedirectUris == null)
            {
                s.PostLogoutRedirectUris = new List<string>();
            }
            if (s.ScopeRestrictions == null)
            {
                s.ScopeRestrictions = new List<string>();
            }
            if (s.IdentityProviderRestrictions == null)
            {
                s.IdentityProviderRestrictions = new List<string>();
            }
            if (s.Claims == null)
            {
                s.Claims = new List<Claim>();
            }
            if (s.CustomGrantTypeRestrictions == null)
            {
                s.CustomGrantTypeRestrictions = new List<string>();
            }
            if (s.AllowedCorsOrigins == null)
            {
                s.AllowedCorsOrigins = new List<string>();
            }

            return Mapper.Map<Models.Client, Entities.Client>(s);
        }
    }
}
