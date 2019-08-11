using paperioBot.Helpers;
using paperioBot.InternalClasses;
using System;
using System.Linq;

namespace paperioBot.Strategies
{
	class GreedyCubeStrategy : Strategy
	{
		public GreedyCubeStrategy() : base() { }

		public GreedyCubeStrategy(WorldTickParams currentTickParams) : base(currentTickParams) {}

		private int[] directionPriority = new[] {0, 2, 1, 3};

		private int priorityIndex = 0;

		private int[] fieldCorrection = new []{0,0,0,0};

		private int[] FindClosestVictimTail(WorldStartParams startParams, State currentState)
		{
			var players = CurrentTickParams.players.Where(p => p.Key != "i");
			var tails = players.SelectMany(p => p.Value.lines);
			return tails.FirstOrDefault(t =>
				(t[0] == currentState.position[0] && Math.Abs(t[1] - currentState.position[1]) == startParams.width) ||
				(t[1] == currentState.position[1] && Math.Abs(t[0] - currentState.position[0]) == startParams.width)
			);
		}
		
		private int FindShortestWayToBorder(WorldStartParams startParams, State currentState)
		{
			var totalWidth = startParams.width * startParams.x_cells_count - fieldCorrection[2];
			var totalHeight = startParams.width * startParams.y_cells_count - fieldCorrection[3];
			var toTop = totalHeight - currentState.position[1];
			var toRight = totalWidth - currentState.position[0];
			var toLeft = currentState.position[0] - fieldCorrection[0];
			var toBottom = currentState.position[1] - fieldCorrection[1];
			if (toTop <= toBottom)
			{
				if (toTop <= toLeft && toTop <= toRight)
				{
					return 2;
				}
				else
				{
					if (toLeft <= toRight)
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
				if (toLeft <= toBottom && toLeft <= toRight)
				{
					return 0;
				}
				else
				{
					if (toBottom <= toRight)
					{
						return 3;
					}
					else
					{
						return 1;
					}
				}
			}
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

		public override int SelectFirstDirection(WorldStartParams startParams, State currentState)
		{
			fieldCorrection = new[] { 0, startParams.width, 0, 0 };
			var way = FindShortestWayToBorder(startParams, currentState);
			ShiftPriorities(way);
			return directionPriority[0];
		}

		public override int SelectDirection(WorldStartParams startParams, State currentState)
		{
			bool isSuicide;
			int index = directionPriority[priorityIndex];
			var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(index), startParams.width, currentState);
			isSuicide = CollisionHelper.CheckDirectionForSuicide(newState, CurrentTickParams, fieldCorrection);
			if (isSuicide)
			{
				priorityIndex = (priorityIndex + 1) % directionPriority.Length;
				return directionPriority[priorityIndex];
			}
			else
			{
				var victimTail = FindClosestVictimTail(startParams, currentState);
				if (victimTail != null)
				{
					fieldCorrection = new[] {0, 0, 0, 0};
					return DirectionHelper.GetDirectionToPosition(currentState, victimTail);
				}
			}

			return directionPriority[priorityIndex];
		}
	}
}
