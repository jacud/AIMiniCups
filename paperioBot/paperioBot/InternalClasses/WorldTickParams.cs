using System.Collections.Generic;

namespace paperioBot.InternalClasses
{
	internal class WorldTickParams : IWorldParam
	{
		public Dictionary<string, Player> players { get; set; }

		public int tick_num { get; set; }
	}
}
