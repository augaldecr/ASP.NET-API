﻿using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace ASP.NET_API.Utilities
{
    public class HeaderAttribute : Attribute, IActionConstraint
    {
        private readonly string _header;
        private readonly string _value;

        public HeaderAttribute(string header, string value)
        {
            _header = header;
            _value = value;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var headers = context.RouteContext.HttpContext.Request.Headers;

            if (!headers.ContainsKey(_header))
                return false;

            return string.Equals(headers[_header], _value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
