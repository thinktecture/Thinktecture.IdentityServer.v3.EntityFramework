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

using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Entities = IdentityServer3.EntityFramework.Entities;

namespace IdentityServer3.Core.Models
{
    public static class EntitiesMap
    {
        public static IMapper Mapper { get; set; }

        static EntitiesMap()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Models.Scope, Entities.Scope>(MemberList.Source)
                    .ForSourceMember(x => x.Claims, opts => opts.Ignore())
                    .ForMember(x => x.ScopeClaims, opts => opts.MapFrom(src => src.Claims.Select(x => x)))
                    .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                config.CreateMap<Models.ScopeClaim, Entities.ScopeClaim>(MemberList.Source);
                config.CreateMap<Models.Secret, Entities.ScopeSecret>(MemberList.Source);

                config.CreateMap<Models.Secret, Entities.ClientSecret>(MemberList.Source);
                config.CreateMap<Models.Client, Entities.Client>(MemberList.Source)
                    .ForMember(x => x.UpdateAccessTokenOnRefresh, opt => opt.MapFrom(src => src.UpdateAccessTokenClaimsOnRefresh))
                    .ForMember(x => x.AllowAccessToAllGrantTypes, opt => opt.MapFrom(src => src.AllowAccessToAllCustomGrantTypes))
                    .ForMember(x => x.AllowedCustomGrantTypes, opt => opt.MapFrom(src => src.AllowedCustomGrantTypes.Select(x => new Entities.ClientCustomGrantType { GrantType = x })))
                    .ForMember(x => x.RedirectUris, opt => opt.MapFrom(src => src.RedirectUris.Select(x => new Entities.ClientRedirectUri { Uri = x })))
                    .ForMember(x => x.PostLogoutRedirectUris, opt => opt.MapFrom(src => src.PostLogoutRedirectUris.Select(x => new Entities.ClientPostLogoutRedirectUri { Uri = x })))
                    .ForMember(x => x.IdentityProviderRestrictions, opt => opt.MapFrom(src => src.IdentityProviderRestrictions.Select(x => new Entities.ClientIdPRestriction { Provider = x })))
                    .ForMember(x => x.AllowedScopes, opt => opt.MapFrom(src => src.AllowedScopes.Select(x => new Entities.ClientScope { Scope = x })))
                    .ForMember(x => x.AllowedCorsOrigins, opt => opt.MapFrom(src => src.AllowedCorsOrigins.Select(x => new Entities.ClientCorsOrigin { Origin = x })))
                    .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => new Entities.ClientClaim { Type = x.Type, Value = x.Value })));
            }).CreateMapper();
        }

        public static Entities.Scope ToEntity(this Models.Scope s)
        {
            if (s == null) return null;

            if (s.Claims == null)
            {
                s.Claims = new List<Models.ScopeClaim>();
            }
            if (s.ScopeSecrets == null)
            {
                s.ScopeSecrets = new List<Models.Secret>();
            }

            return Mapper.Map<Models.Scope, Entities.Scope>(s);
        }

        public static Entities.Client ToEntity(this Models.Client s)
        {
            if (s == null) return null;

            if (s.ClientSecrets == null)
            {
                s.ClientSecrets = new List<Secret>();
            }
            if (s.RedirectUris == null)
            {
                s.RedirectUris = new List<string>();
            }
            if (s.PostLogoutRedirectUris == null)
            {
                s.PostLogoutRedirectUris = new List<string>();
            }
            if (s.AllowedScopes == null)
            {
                s.AllowedScopes = new List<string>();
            }
            if (s.IdentityProviderRestrictions == null)
            {
                s.IdentityProviderRestrictions = new List<string>();
            }
            if (s.Claims == null)
            {
                s.Claims = new List<Claim>();
            }
            if (s.AllowedCustomGrantTypes == null)
            {
                s.AllowedCustomGrantTypes = new List<string>();
            }
            if (s.AllowedCorsOrigins == null)
            {
                s.AllowedCorsOrigins = new List<string>();
            }

            return Mapper.Map<Models.Client, Entities.Client>(s);
        }
    }
}
