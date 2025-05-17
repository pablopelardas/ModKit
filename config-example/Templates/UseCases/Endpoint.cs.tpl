using {{Namespace}};
using CakeFlow.Modules.{{Module}}.Domain.Abstractions;
using CakeFlow.Modules.Supplies.Presentation.ApiResponses;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CakeFlow.Modules.{{Module}}.Presentation.{{Submodule}};

internal static class {{UseCase}}
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{{Submodule}}/{{UseCase}}", async (ISender sender) =>
        {
            var request = new {{UseCase}}{{TypeCapital}}();
            Result<Guid> result = await sender.Send(request);
            return result.Match(Results.Ok, ApiResults.Problem);
        }).WithTags(Tags.{{Submodule}});
    }
}
