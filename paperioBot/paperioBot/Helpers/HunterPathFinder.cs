using paperioBot.InternalClasses;
using System;
using System.Linq;

namespace paperioBot.Helpers
{
	internal class HunterPathFinder
	{
		private WorldStartParams _startParams;

		private int[][] battleField;

		private int[][] weights;

		private int[] _me;

		private WorldTickParams _currentTickParam;

		public HunterPathFinder(WorldStartParams startParams, WorldTickParams currentTickParam)
		{
			_startParams = startParams;
			_currentTickParam = currentTickParam;
			GenerateNewBattleField();
		}

		private void GenerateNewBattleField()
		{

			battleField = new int[_startParams.x_cells_count][];
			for (int i = 0; i < _startParams.x_cells_count; i++)
			{
				battleField[i] = new int[_startParams.y_cells_count];
			}

			var me = _currentTickParam.players["i"];
			_me = new[] { me.position[0] / _startParams.width, me.position[1] / _startParams.width };

			battleField[_me[0]][_me[1]] = (int)CellTypes.Me;

			foreach (var position in me.lines)
			{
				battleField[position[0] / _startParams.width][position[1] / _startParams.width] =
					(int)CellTypes.MyTail;
			}

			foreach (var position in me.territory)
			{
				battleField[position[0] / _startParams.width][position[1] / _startParams.width] =
					(int)CellTypes.MyTerritory;
			}

			foreach (var player in _currentTickParam.players.Where(p => p.Key != "i"))
			{
				var enemyPosition = player.Value.position;
				battleField[enemyPosition[0] / _startParams.width][enemyPosition[1] / _startParams.width] =
					(int)CellTypes.Opponent;

				foreach (var position in player.Value.lines)
				{
					battleField[position[0] / _startParams.width][position[1] / _startParams.width] =
						(int)CellTypes.OpponentsTail;
				}

				foreach (var position in player.Value.territory)
				{
					battleField[position[0] / _startParams.width][position[1] / _startParams.width] =
						(int)CellTypes.OpponentTerritory;
				}
			}
		}

		private int FindDeathLimit()
		{
			int limit = int.MaxValue;
			var me = _currentTickParam.players["i"];
			if (!me.lines.Any())
			{
				return limit;
			}
			foreach (var keyValuePair in _currentTickParam.players.Where(p => p.Key != "i"))
			{
				var player = keyValuePair.Value;
				var playerPosition = new[]{player.position[0] / _startParams.width, player.position[1] / _startParams.width};
				foreach (var tail in me.lines)
				{
					var isMyTerritory = me.territory.Any(ter => ter[0] == tail[0] && ter[1]==tail[1]);
					var distance = isMyTerritory? limit: Math.Abs(tail[0] / _startParams.width - playerPosition[0]) + Math.Abs(tail[1] / _startParams.width - playerPosition[1]) - 1;
					limit = Math.Min(limit, distance);
				}
			}

			return limit;
		}

		private void SetWeightToNeightbour(int startX, int startY, int limit, int forbiddenWay)
		{
			var currentWeight = weights[startX][startY];
			if (currentWeight >= limit || battleField[startX][startY] == (int)CellTypes.MyTerritory)
			{
				return;
			}
			var forbiddenTypes = new int[]
			{
				(int)CellTypes.MyTail,
				(int)CellTypes.Opponent,
			};
			//left
			if (startX > 0 && !forbiddenTypes.Contains(battleField[startX - 1][startY]) && forbiddenWay != 0)
			{
				var newWeight = weights[startX - 1][startY];
				if (newWeight == 0 || newWeight > currentWeight + 1)
				{
					weights[startX - 1][startY] = newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					SetWeightToNeightbour(startX - 1, startY, limit, -1);
				}
			}
			//right
			if (startX < _startParams.x_cells_count - 1 && !forbiddenTypes.Contains(battleField[startX + 1][startY]) && forbiddenWay != 1)
			{
				var newWeight = weights[startX + 1][startY];
				if (newWeight == 0 || newWeight > currentWeight + 1)
				{
					weights[startX + 1][startY] =
						newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					SetWeightToNeightbour(startX + 1, startY, limit, -1);
				}
			}
			//up
			if (startY < _startParams.y_cells_count - 1 && !forbiddenTypes.Contains(battleField[startX][startY + 1]) && forbiddenWay != 2)
			{
				var newWeight = weights[startX][startY + 1];
				if (newWeight == 0 || newWeight > currentWeight + 1)
				{
					weights[startX][startY + 1] =
						newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					SetWeightToNeightbour(startX, startY + 1, limit, -1);
				}
			}
			//down
			if (startY > 0 && !forbiddenTypes.Contains(battleField[startX][startY - 1]) && forbiddenWay != 3)
			{
				var newWeight = weights[startX][startY - 1];
				if (newWeight == 0 || newWeight > currentWeight + 1)
				{
					weights[startX][startY - 1] =
						newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					SetWeightToNeightbour(startX, startY - 1, limit, -1);
				}
			}
		}

		private int[] RollBack(int[] pos)
		{
			var ways = new int[1];
			if (pos == null) return ways;
			int[] newPos = new int[2];
			var currentWeight = weights[pos[0]][pos[1]];
			if (currentWeight == 0) return ways;
			//left
			if (pos[0] > 0 && weights[pos[0] -1][pos[1]] == currentWeight-1)
			{
				ways[0] = 1;
				newPos = new[] {pos[0] - 1, pos[1]};
			} else
				//right
			if (pos[0] < _startParams.x_cells_count - 1 && weights[pos[0] + 1][pos[1]] == currentWeight - 1)
			{
				ways[0] = 0;
				newPos = new[] { pos[0] + 1, pos[1] };
			}
			else
				//up
			if (pos[1] < _startParams.y_cells_count - 1 && weights[pos[0]][pos[1] + 1] == currentWeight - 1)
			{
				ways[0] = 3;
				newPos = new[] { pos[0], pos[1] + 1 };
			}
			else
			//down
			if (pos[1] > 0 && weights[pos[0]][pos[1] - 1] == currentWeight - 1)
			{
				ways[0] = 2;
				newPos = new[] { pos[0], pos[1] - 1 };
			}
			return ways.Concat(RollBack(newPos)).ToArray();
		}

		public int[] BuildWeightsAndReturnWay()
		{
			var maxLength = FindDeathLimit();
			weights = new int[_startParams.x_cells_count][];
			for (int i = 0; i < _startParams.x_cells_count; i++)
			{
				weights[i] = new int[_startParams.y_cells_count];
			}

			SetWeightToNeightbour(_me[0], _me[1], maxLength, DirectionHelper.Way(_currentTickParam.players["i"].direction));

			var goodTerritories = _currentTickParam.players["i"].territory;
			goodTerritories = goodTerritories
				.Where(t => weights[t[0] / _startParams.width][t[1] / _startParams.width] > 0)
				.OrderBy(t => weights[t[0] / _startParams.width][t[1] / _startParams.width])
				.Select(t => new []
				{
					t[0] / _startParams.width,
					t[1] / _startParams.width,
					weights[t[0] / _startParams.width][t[1] / _startParams.width]
				});
			var selectedPathEnd = goodTerritories.FirstOrDefault();
			return RollBack(selectedPathEnd);
		}

		public int[] FindClosestVictimCell(State currentState, WorldTickParams param)
		{
			return new int[]{0};
		}
	}
}
