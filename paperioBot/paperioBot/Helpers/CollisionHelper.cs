using System;
using paperioBot.InternalClasses;
using System.Linq;

namespace paperioBot.Helpers
{
	internal static class CollisionHelper
	{
		#region Properties

		private static WorldStartParams _startParams;

		#endregion


		public static void SetStartParams(WorldStartParams startParams)
		{
			_startParams = startParams;
		}

		public static bool CheckTailCollision(State currentState, WorldTickParams currentParams)
		{
			if (currentParams == null)
			{
				return false;
			}
			var tailBlocks = currentParams.players["i"].lines.Where(block =>
			{
				return (
					block[0] == currentState.position[0] &&
					block[1] == currentState.position[1]
				);
			});
			var railVlocksCount = tailBlocks.Count();
			return tailBlocks.Any();
		}

		public static bool CheckBorderCollision(State currentState, int[] fieldCorrection)
		{
			if (currentState == null)
			{
				return false;
			}

			if (currentState.position[0] <= 0 + fieldCorrection[0] || currentState.position[1] <= fieldCorrection[1] +_startParams.speed)
			{
				return true;
			}

			if (currentState.position[0] >= _startParams.x_cells_count* _startParams.width - fieldCorrection[2] || currentState.position[1] >= _startParams.y_cells_count* _startParams.width - fieldCorrection[3])
			{
				return true;
			}
			return false;
		}

		public static bool CheckDirectionForSuicide(State currentState, WorldTickParams currentParams, int[] fieldCorrection = null)
		{
			var correction = fieldCorrection != null ? fieldCorrection : new[] {0, 0, 0, 0};
			return CheckBorderCollision(currentState, correction) || CheckTailCollision(currentState, currentParams);
		}

		public static bool CheckIsInHome(State currentState, WorldTickParams tickParams)
		{
			return tickParams.players["i"].territory.FirstOrDefault(x => x[0] == currentState.position[0] && x[1] == currentState.position[1]) != null;
		}
	}
}
