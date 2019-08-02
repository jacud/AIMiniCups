using System.Collections.Generic;

namespace paperioBot.InternalClasses
{
	internal class WorldParams
	{
		public Dictionary<string,Player> players { get; set; }

		public int tick_num { get; set; }

		public int x_cells_count { get; set; }

		public int y_cells_count { get; set; }

		public int speed { get; set; }

		public int width { get; set; }
	}
}
