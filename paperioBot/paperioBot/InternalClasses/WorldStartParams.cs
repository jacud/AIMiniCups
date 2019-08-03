namespace paperioBot.InternalClasses
{
	internal class WorldStartParams: IWorldParam
	{
		public int x_cells_count { get; set; }

		public int y_cells_count { get; set; }

		public int speed { get; set; }

		public int width { get; set; }
	}
}
