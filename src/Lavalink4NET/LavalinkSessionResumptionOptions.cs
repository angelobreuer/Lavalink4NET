namespace Lavalink4NET;

using System;
using System.Diagnostics.CodeAnalysis;

public readonly record struct LavalinkSessionResumptionOptions
{
	public TimeSpan? Timeout { get; }

	[MemberNotNullWhen(true, nameof(Timeout))]
	public bool IsEnabled => Timeout is not null;

	public LavalinkSessionResumptionOptions(TimeSpan? timeout = null)
	{
		if (timeout is not null && timeout.Value < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(
				paramName: nameof(timeout),
				actualValue: timeout,
				message: "The timeout must be greater than or equal to zero.");
		}

		Timeout = timeout;
	}
}
