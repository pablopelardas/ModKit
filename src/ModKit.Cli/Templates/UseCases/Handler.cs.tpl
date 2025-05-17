using {{MessagingNamespace}};
using {{DomainNamespace}};

namespace {{Namespace}};

internal sealed class {{UseCase}}CommandHandler : ICommandHandler<{{UseCase}}Command, Guid>
{
    public Task<Result<Guid>> Handle({{UseCase}}Command request, CancellationToken cancellationToken)
    {
        // logic
        return Task.FromResult(Result.Success(Guid.NewGuid()));
    }
}
