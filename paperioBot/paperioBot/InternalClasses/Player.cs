using System.Collections.Generic;

namespace paperioBot.InternalClasses
{
	internal class Player
	{
		public int score { get; set; }

		public IEnumerable<int[]> lines { get; set; }

		public IEnumerable<int[]> territory { get; set; }

		public int[] position { get; set; }

		public string direction { get; set; }
	}
}
