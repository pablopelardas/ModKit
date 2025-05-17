using {{MessagingNamespace}};
using {{DomainNamespace}};

namespace {{Namespace}};

internal sealed class {{UseCase}}QueryHandler : IQueryHandler<{{UseCase}}Query, Guid>
{
    public Task<Result<Guid>> Handle({{UseCase}}Query request, CancellationToken cancellationToken)
    {
        // logic
        return Task.FromResult(Result.Success(Guid.NewGuid()));
    }
}
