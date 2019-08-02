using System.Collections.Generic;

namespace paperioBot.InternalClasses
{
	internal class Player
	{
		public int score { get; set; }

		public IEnumerable<Position> lines { get; set; }

		public Position position { get; set; }

		public string direction { get; set; }
	}
}
