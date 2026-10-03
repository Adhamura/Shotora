namespace Shotora.App.Models.Updates;

/// <summary>
///     Lenient semantic-version helpers for release tags such as "v1.2.3", "1.0" or "2.0.0-beta.1".
/// </summary>
public static class VersionComparer
{
	public static bool TryParse(string? text, out Version version, out string? prerelease)
	{
		version    = new Version(0, 0, 0);
		prerelease = null;

		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		var value = text.Trim();
		if (value.StartsWith('v') || value.StartsWith('V'))
		{
			value = value[1..];
		}

		var plusIndex = value.IndexOf('+');
		if (plusIndex >= 0)
		{
			value = value[..plusIndex];
		}

		var dashIndex = value.IndexOf('-');
		if (dashIndex >= 0)
		{
			prerelease = value[(dashIndex + 1)..];
			value      = value[..dashIndex];
		}

		var parts = value.Split('.');
		if (parts.Length is < 1 or > 4)
		{
			return false;
		}

		var numbers = new int[4];
		for (var i = 0; i < parts.Length; i++)
		{
			if (!int.TryParse(parts[i], out var number) || number < 0)
			{
				return false;
			}

			numbers[i] = number;
		}

		version = new Version(numbers[0], numbers[1], numbers[2], numbers[3]);
		return true;
	}

	/// <summary>
	///     Returns true when <paramref name="candidate" /> is strictly newer than <paramref name="current" />.
	///     Unparseable input never counts as newer.
	/// </summary>
	public static bool IsNewer(string? candidate, string? current)
	{
		if (!TryParse(candidate, out var candidateVersion, out var candidatePre) ||
		    !TryParse(current, out var currentVersion, out var currentPre))
		{
			return false;
		}

		var comparison = candidateVersion.CompareTo(currentVersion);
		if (comparison != 0)
		{
			return comparison > 0;
		}

		// Same numeric version: a stable release is newer than any prerelease of it.
		if (candidatePre == null)
		{
			return currentPre != null;
		}

		return currentPre != null && ComparePrerelease(candidatePre, currentPre) > 0;
	}

	/// <summary>SemVer 2.0 precedence for prerelease labels: numeric identifiers compare numerically ("beta.10" &gt; "beta.9").</summary>
	private static int ComparePrerelease(string left, string right)
	{
		var leftParts  = left.Split('.');
		var rightParts = right.Split('.');
		for (var i = 0; i < Math.Min(leftParts.Length, rightParts.Length); i++)
		{
			var leftNumeric  = int.TryParse(leftParts[i],  out var leftNumber);
			var rightNumeric = int.TryParse(rightParts[i], out var rightNumber);
			var comparison = (leftNumeric, rightNumeric) switch
			{
				(true, true)  => leftNumber.CompareTo(rightNumber),
				(true, false) => -1,
				(false, true) => 1,
				_             => string.CompareOrdinal(leftParts[i], rightParts[i])
			};

			if (comparison != 0)
			{
				return comparison;
			}
		}

		return leftParts.Length.CompareTo(rightParts.Length);
	}

	public static string Normalize(string? text)
	{
		if (!TryParse(text, out var version, out var prerelease))
		{
			return text?.Trim() ?? string.Empty;
		}

		var core = version.Revision > 0
			? $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}"
			: $"{version.Major}.{version.Minor}.{version.Build}";
		return prerelease == null ? core : $"{core}-{prerelease}";
	}
}
