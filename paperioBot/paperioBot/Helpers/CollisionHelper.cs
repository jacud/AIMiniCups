using paperioBot.InternalClasses;
using System;
using System.Linq;

namespace paperioBot.Helpers
{
	internal static class CollisionHelper
	{
		#region Properties

		private static WorldStartParams _startParams;

		private static Random random = new Random();

		#endregion


		public static void SetStartParams(WorldStartParams startParams)
		{
			_startParams = startParams;
		}

		private static bool CheckTailCollision(State currentState, GameParams<WorldTickParams> currentParams)
		{
			if (currentParams == null)
			{
				return false;
			}
			var tailBlocks = currentParams.@params.players["i"].lines.Where(block =>
			{
				return (
					(block[0] + _startParams.width > currentState.position[0] && block[0] - _startParams.width  < currentState.position[0]) &&
					(block[1] + _startParams.width > currentState.position[1] && block[1] - _startParams.width  < currentState.position[1])
				);
			});
			return tailBlocks.Any();
		}

		private static bool CheckBorderCollision(State currentState)
		{
			if (currentState == null)
			{
				return false;
			}

			if (currentState.position[0] < 0 || currentState.position[1] < 0)
			{
				return true;
			}

			if (currentState.position[0] > _startParams.x_cells_count* _startParams.width || currentState.position[1] > _startParams.y_cells_count* _startParams.width)
			{
				return true;
			}
			return false;
		}

		private static bool CheckDirectionForSuicide(State currentState, GameParams<WorldTickParams> currentParams)
		{
			return CheckBorderCollision(currentState) || CheckTailCollision(currentState, currentParams);
		}

		public static string FindNewDirection(State currentState, GameParams<WorldTickParams> currentParams)
		{
			int index = random.Next(0, DirectionHelper.DirectionsCount);
			if (currentState != null && currentParams != null)
			{
				var isSuicide = true;
				while (isSuicide)
				{
					var newState = MotionHelper.MoveToDirection(DirectionHelper.Direction(index), _startParams.width,
						currentState);
					isSuicide = CheckDirectionForSuicide(newState, currentParams);
					if (isSuicide)
					{
						index = random.Next(0, DirectionHelper.DirectionsCount - 2) - (index > 1 ? 2 : - 2);
					}
					
				}
			}

			return DirectionHelper.Direction(index);
		}
	}
}
