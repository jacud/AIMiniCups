﻿using paperioBot.InternalClasses;

namespace paperioBot.Helpers
{
	internal static class MotionHelper
	{
		public static State MoveToDirection(string direction, int stepSize, State currentState)
		{
			var newState = new State(currentState);
			var way = DirectionHelper.Way(direction);
			var currentStateWay = DirectionHelper.Way(currentState.direction);
			var correctionDeltas = DirectionHelper.CorrectionDeltas;

			if (currentStateWay < 2)
			{
				newState.position[0] += correctionDeltas[0];
			}
			else
			{
				newState.position[1] -= correctionDeltas[1];
			}

			if (DirectionHelper.CheckIsDirectionsComplanar(way, currentStateWay))
			{
				way = currentStateWay;
			}

			if (way < 2)
			{
				newState.position[0] += (way * 2 - 1) * stepSize;
			}
			else
			{
				newState.position[1] -= (way * 2 - 5) * stepSize;
			}
			newState.direction = DirectionHelper.Direction(way);
			return newState;
		}
	}
}
