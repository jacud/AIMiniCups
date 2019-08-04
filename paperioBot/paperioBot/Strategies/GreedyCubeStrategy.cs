using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;

namespace paperioBot.Strategies
{
	class GreedyCubeStrategy : Strategy
	{
		public GreedyCubeStrategy() : base() { }

		public GreedyCubeStrategy(WorldTickParams currentTickParams) : base(currentTickParams) {}

		private bool isOnWayToHome = false;

		private int[] directionPriority = new[] {0, 2, 1, 3, 0, 2, 1};

		private int priorityIndex = 0;

		private bool skipStep = false;

		private int FindShortestWayToBorder(WorldStartParams startParams, State currentState)
		{
			var totalWidth = startParams.width * startParams.x_cells_count;
			var totalHeight = startParams.width * startParams.y_cells_count;
			var toTop = totalHeight - currentState.position[1];
			var toRight = totalWidth - currentState.position[0];
			if (toTop <= currentState.position[1])
			{
				if (toTop <= currentState.position[0] && toTop <= toRight)
				{
					return 2;
				}
				else
				{
					if (currentState.position[0] <= toRight)
					{
						return 0;
					}
					else
					{
						return 1;
					}
				}
			}
			else
			{
				if (currentState.position[0] <= currentState.position[1] && currentState.position[0] <= toRight)
				{
					return 0;
				}
				else
				{
					if (currentState.position[1] <= toRight)
					{
						return 3;
					}
					else
					{
						return 1;
					}
				}
			}

			throw new ArgumentOutOfRangeException();
		}

		private void ShiftPriorities(int currenWay)
		{
			var shifMask = Array.FindIndex(directionPriority, x => x == currenWay);
			var newPriories = new int[directionPriority.Length];
			for (int i = 0; i < directionPriority.Length; i++)
			{
				newPriories[i] = directionPriority[(i+shifMask) % directionPriority.Length];
			}

			directionPriority = newPriories;
		}

		public override int SelectDirection(WorldStartParams startParams, State currentState, WorldTickParams currentParams)
		{
			var currentDirection = currentState.direction;
			if (String.IsNullOrEmpty(currentDirection))
			{
				var way = FindShortestWayToBorder(startParams, currentState);
				ShiftPriorities(way);
				return directionPriority[0];
			}

			var isSuicide = true;
			int index = directionPriority[priorityIndex];
			if (skipStep)
			{
				skipStep = false;
				return index;
			}
			var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(index), startParams.width, currentState);
			isSuicide = CollisionHelper.CheckDirectionForSuicide(newState, currentParams);
			if (isSuicide)
			{
				skipStep = true;
				return directionPriority[++priorityIndex];
			}

			return directionPriority[priorityIndex];
		}
	}
}
