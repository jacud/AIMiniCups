using paperioBot.InternalClasses;

namespace paperioBot.Strategies
{
	internal abstract class Strategy
	{
		protected WorldTickParams CurrentTickParams;

		public void MutateStates(WorldTickParams currentTickParams)
		{
			CurrentTickParams = currentTickParams;
		}

		public Strategy() {}

		public Strategy(WorldTickParams currentTickParams)
		{
			CurrentTickParams = currentTickParams;
		}

		public abstract int SelectDirection(WorldStartParams straParams, State currentState);

		public abstract int SelectFirstDirection(WorldStartParams straParams, State currentState);
	}
}
