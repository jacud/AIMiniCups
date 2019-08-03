using System;

namespace paperioBot.Helpers
{
	internal static class DirectionHelper
	{
		private static string[] directions = new[] { "left", "right", "up", "down" };

		public static int Way(string direction)
		{
			return Array.IndexOf(directions,direction);
		}

		public static string Direction(int way)
		{
			return directions[way];
		}

		public static int DirectionsCount => directions.Length;
	}
}
