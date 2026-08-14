using MediatR;
using NoCap.Eats.Identity.Application.Interfaces;

namespace NoCap.Eats.Identity.Application.Commands.RevokeToken;

public class RevokeTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepo) : IRequestHandler<RevokeTokenCommand>
{
    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        await refreshTokenRepo.RevokeAllForUserAsync(request.UserId, cancellationToken);
        await refreshTokenRepo.SaveChangesAsync(cancellationToken);
    }
}
