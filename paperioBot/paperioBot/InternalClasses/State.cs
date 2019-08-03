namespace paperioBot.InternalClasses
{
	internal class State
	{
		public int[] position;

		public string direction;

		public State()
		{
		}

		public State(State oldState)
		{
			position = (int[])oldState.position.Clone();
			direction = oldState.direction;
		}
	}
}
