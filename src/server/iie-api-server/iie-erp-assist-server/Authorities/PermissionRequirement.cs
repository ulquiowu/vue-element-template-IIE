using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Authorities
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string _permissionName { get; }
        public PermissionRequirement(string PermissionName)
        {
            _permissionName = PermissionName;
        }
    }
}
