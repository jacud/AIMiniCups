using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;
using System.Collections.Generic;

namespace paperioBot.Strategies
{
	class GreedyStrategy : Strategy
	{
		private static Random random = new Random();

		public GreedyStrategy() : base() { }

		public GreedyStrategy(WorldTickParams currentTickParams) : base(currentTickParams) {}

		private bool isOnWayToHome = false;

		public override int SelectDirection(WorldStartParams startParams, State currentState, WorldTickParams currentParams)
		{
			throw new NotImplementedException();
			int index = random.Next(0, DirectionHelper.DirectionsCount);
			if (currentState != null && currentParams != null)
			{
				var forbiddenDirections = new List<int>();
				var isSuicide = true;
				while (isSuicide)
				{
					var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(index), startParams.width, currentState);
					isSuicide = CollisionHelper.CheckDirectionForSuicide(newState, currentParams);
					if (isSuicide)
					{
						forbiddenDirections.Add(index);
						while (forbiddenDirections.Contains(index))
						{
							index = random.Next(0, DirectionHelper.DirectionsCount);
						}
					}

				}
			}

			return index;
		}

		private int[] FindVictimCell(WorldStartParams startParams, State currentState, WorldTickParams currentParams)
		{

			return null;
		}
	}
}
