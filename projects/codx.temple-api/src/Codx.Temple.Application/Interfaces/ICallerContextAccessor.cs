namespace Codx.Temple.Application.Interfaces;

using Codx.Temple.Application.DTOs.Auth;

public interface ICallerContextAccessor
{
    CallerContextDto? GetCallerContext();
}
