using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Authentication;
internal sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions authenticationOptions;

    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    {
        this.authenticationOptions = authenticationOptions.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = this.authenticationOptions.Audience;
        options.MetadataAddress = this.authenticationOptions.MetadataUrl;
        options.RequireHttpsMetadata = this.authenticationOptions.RequireHttpsMetadata;
        options.TokenValidationParameters.ValidIssuer = this.authenticationOptions.Issuer;
    }
}
