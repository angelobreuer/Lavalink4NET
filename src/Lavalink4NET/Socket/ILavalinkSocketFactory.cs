namespace Lavalink4NET.Socket;
using Microsoft.Extensions.Options;

public interface ILavalinkSocketFactory
{
    ILavalinkSocket Create(IOptions<LavalinkSocketOptions> options);
}