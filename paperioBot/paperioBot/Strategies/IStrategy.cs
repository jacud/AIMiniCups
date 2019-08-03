using paperioBot.InternalClasses;

namespace paperioBot.Strategies
{
	internal abstract class Strategy
	{
		private WorldTickParams _currentTickParams;

		public void MutateStates(WorldTickParams currentTickParams)
		{
			_currentTickParams = currentTickParams;
		}

		public Strategy() {}

		public Strategy(WorldTickParams currentTickParams)
		{
			_currentTickParams = currentTickParams;
		}

		public abstract int SelectDirection(WorldStartParams straParams, State currentState, WorldTickParams currentTickParams);
	}
}
