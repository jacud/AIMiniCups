using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;
using System.Linq;

namespace paperioBot.Strategies
{
	class SuicideStrategy : Strategy
	{
		public SuicideStrategy() : base() { }

		public SuicideStrategy(WorldTickParams currentTickParams) : base(currentTickParams) {}

		private int[] directionPriority = new[] {0, 0,0,0,0, 2, 1, 3,3,3,3,3,3};

		private int priorityIndex = 0;


		public override int SelectFirstDirection(WorldStartParams startParams, State currentState)
		{
			return directionPriority[0];
		}

		public override int SelectDirection(WorldStartParams startParams, State currentState)
		{
			if (currentState.direction == null)
			{
				return directionPriority[priorityIndex - 1];
			}
			var newWay = directionPriority[priorityIndex++];
			var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(newWay), startParams.width, currentState);
			var isSuicide = CollisionHelper.CheckDirectionForSuicide(newState, CurrentTickParams);
			return newWay;
		}
	}
}
