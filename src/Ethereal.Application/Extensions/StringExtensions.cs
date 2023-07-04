using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ethereal.Application.Extensions;

public static class StringExtensions
{
	public static bool ContainsTimespan(this string str, out TimeSpan timespan)
	{
		var parts = str.Split();

		foreach (var part in parts)
		{
			// Add a regular expression that matches a timestamp preceded by a number and a period, with or without a space
			var regex = new Regex(@"(\d+\.\s*)?(\d{1,2}:\d{2}(:\d{2})?)");
			var match = regex.Match(part);

			if (match.Success)
			{
				var cleaned = match.Groups[2].Value;  // The second group in the regex is the timestamp

				if (cleaned.TryParseToTimeSpan(out timespan))
					return true;
			}
		}

		timespan = TimeSpan.MaxValue;
		return false;
	}

    public static string RemoveTimespan(this string str)
    {
        var stringBuilder = new StringBuilder();
        var parts = str.Split();

        foreach (var part in parts)
            if (part.ContainsTimespan(out _) == false)
                stringBuilder.Append(part + " ");

        return stringBuilder.ToString().Trim();
    }

	public static string RemoveIllegalCharacters(this string str)
	{
		var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + "\"~|";
		return invalid.Aggregate(str, (current, c) => current.Replace(c.ToString(), string.Empty));
	}

}
