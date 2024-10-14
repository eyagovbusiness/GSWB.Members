using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TGF.Common.ROP.Errors;

namespace Members.Domain.Errors
{

    public static partial class DomainErrors
    {
        public static class Role
        {
            public static HttpError NotFoundIdList => new(
            new Error("DomainErrors.Role.NotFoundIdList",
                $"One or more roles were not found from the provided roleId list."),
            HttpStatusCode.NotFound);
        }
    }
}
