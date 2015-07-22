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
using System.Linq;
using System.Security.Claims;

namespace Thinktecture.IdentityServer.EntityFramework.Entities
{
    public static class EntitiesMap
    {
        static EntitiesMap()
        {
            Mapper.CreateMap<Entities.Scope, Thinktecture.IdentityServer.Core.Models.Scope>(MemberList.Destination)
                .ForMember(x => x.Claims, opts => opts.MapFrom(src => src.ScopeClaims.Select(x => x)));
            Mapper.CreateMap<Entities.ScopeClaim, Thinktecture.IdentityServer.Core.Models.ScopeClaim>(MemberList.Destination);

            Mapper.CreateMap<Entities.ClientSecret, Thinktecture.IdentityServer.Core.Models.ClientSecret>(MemberList.Destination);
            Mapper.CreateMap<Entities.Client, Thinktecture.IdentityServer.Core.Models.Client>(MemberList.Destination)
                .ForMember(x => x.CustomGrantTypeRestrictions, opt => opt.MapFrom(src => src.CustomGrantTypeRestrictions.Select(x => x.GrantType)))
                .ForMember(x => x.RedirectUris, opt => opt.MapFrom(src => src.RedirectUris.Select(x => x.Uri)))
                .ForMember(x => x.PostLogoutRedirectUris, opt => opt.MapFrom(src => src.PostLogoutRedirectUris.Select(x => x.Uri)))
                .ForMember(x => x.IdentityProviderRestrictions, opt => opt.MapFrom(src => src.IdentityProviderRestrictions.Select(x => x.Provider)))
                .ForMember(x => x.ScopeRestrictions, opt => opt.MapFrom(src => src.ScopeRestrictions.Select(x => x.Scope)))
                .ForMember(x => x.AllowedCorsOrigins, opt => opt.MapFrom(src => src.AllowedCorsOrigins.Select(x => x.Origin)))
                .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => new Claim(x.Type, x.Value))));
        }

        public static Thinktecture.IdentityServer.Core.Models.Scope ToModel(this Entities.Scope s)
        {
            if (s == null) return null;
            return Mapper.Map<Entities.Scope, Thinktecture.IdentityServer.Core.Models.Scope>(s);
        }

        public static Thinktecture.IdentityServer.Core.Models.Client ToModel(this Entities.Client s)
        {
            if (s == null) return null;
            return Mapper.Map<Entities.Client, Thinktecture.IdentityServer.Core.Models.Client>(s);
        }
    }
}
