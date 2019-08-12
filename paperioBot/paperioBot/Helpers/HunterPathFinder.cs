using paperioBot.InternalClasses;
using System;
using System.Collections.Generic;
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

			foreach (var player in _currentTickParam.players.Where(p => p.Key != "i"))
			{
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

				var enemyPosition = player.Value.position;
				battleField[enemyPosition[0] / _startParams.width][enemyPosition[1] / _startParams.width] =
					(int)CellTypes.Opponent;
			}

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

			battleField[_me[0]][_me[1]] = (int)CellTypes.Me;
		}

		private int FindDeathLimit()
		{
			var lifeTimeLimit = (1500 - _currentTickParam.tick_num) / _startParams.speed;
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
					var distance = isMyTerritory? limit: Math.Abs(tail[0] / _startParams.width - playerPosition[0]) + Math.Abs(tail[1] / _startParams.width - playerPosition[1])-1;
					limit = Math.Min(limit, distance);
				}

				{
					var isMyTerritory = me.territory.Any(ter => ter[0] == me.position[0] && ter[1] == me.position[1]);
					var distanceToHead = isMyTerritory
						? Int32.MaxValue
						: Math.Abs(_me[0] - playerPosition[0]) +
						  Math.Abs(_me[1] - playerPosition[1]);
					limit = Math.Min(limit, distanceToHead);
				}
			}

			return Math.Min(limit, lifeTimeLimit);
		}

		private void SetWeightToNeightbourForHunt(int startX, int startY, int limit, int forbiddenWay)
		{
			var currentWeight = weights[startX][startY];
			if (currentWeight >= limit || battleField[startX][startY] == (int)CellTypes.MyTerritory)
			{
				return;
			}
			var forbiddenTypes = new int[]
			{
				(int)CellTypes.MyTail,
				(int)CellTypes.Me
			};
			//left
			if (startX > 0 && !forbiddenTypes.Contains(battleField[startX - 1][startY]) && forbiddenWay != 0)
			{
				var newWeight = weights[startX - 1][startY];
				if (newWeight == 0 || newWeight > currentWeight + 1 )
				{
					weights[startX - 1][startY] = newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					SetWeightToNeightbourForHunt(startX - 1, startY, limit, -1);
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
					SetWeightToNeightbourForHunt(startX + 1, startY, limit, -1);
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
					SetWeightToNeightbourForHunt(startX, startY + 1, limit, -1);
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
					SetWeightToNeightbourForHunt(startX, startY - 1, limit, -1);
				}
			}
		}

		private Player FindCloasestOpponent(int x, int y)
		{
			var opponents = _currentTickParam.players.Where(p => p.Key != "i");
			if (!opponents.Any())
			{
				return null;
			}

			return opponents.OrderBy(p => Math.Abs(p.Value.position[0] / _startParams.width - x) + Math.Abs(p.Value.position[1] / _startParams.width - y)).FirstOrDefault().Value;
		}

		private int[] SetWeightToNeightbourForAssult(int startX, int startY, int forbiddenWay)
		{
			var currentWeight = weights[startX][startY];
			int[] localBestResult = new[] { startX, startY, currentWeight };
			List<int[]> results = new List<int[]>();
			results.Add(new[] { 1000, 1000, 1000 });
			
			var allowedTypesTypes = new int[]
			{
				(int)CellTypes.MyTerritory,
				(int)CellTypes.Me,
			};

			var finishTypes = new int[]
			{
				(int)CellTypes.EmptyCell,
				(int)CellTypes.OpponentTerritory,
				(int)CellTypes.OpponentsTail,
				(int)CellTypes.Opponent,
			};

			if (finishTypes.Contains(battleField[startX][startY]))
			{
				var clossestOpponent = FindCloasestOpponent(startX, startY);
				if (clossestOpponent == null)
				{
					return localBestResult;
				}

				if (Math.Abs(clossestOpponent.position[0] /_startParams.width - startX) + Math.Abs(clossestOpponent.position[1] / _startParams.width - startY) <=
				    4 && clossestOpponent.lines.Count() == 0)
				{
					localBestResult[2] = 1000;
					return localBestResult;
				}

				return localBestResult;
			}

			if (battleField[startX][startY] == (int)CellTypes.Opponent)
			{
				var clossestOpponent = FindCloasestOpponent(startX, startY);
				if (clossestOpponent.lines.Count() == 0)
				{
					return new[] { startX, startY, 1000 };
				}
			}

			//left
			if (startX > 0 && allowedTypesTypes.Contains(battleField[startX][startY]) && forbiddenWay != 0)
			{
				var newWeight = weights[startX - 1][startY];
				if (newWeight == 0 || (newWeight > currentWeight + 1 && newWeight != Int32.MaxValue))
				{
					weights[startX - 1][startY] = newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					results.Add(SetWeightToNeightbourForAssult(startX - 1, startY, 1));
				}
			}
			//right
			if (startX < _startParams.x_cells_count - 1 && allowedTypesTypes.Contains(battleField[startX][startY]) && forbiddenWay != 1)
			{
				var newWeight = weights[startX + 1][startY];
				if (newWeight == 0 || (newWeight > currentWeight + 1 && newWeight != Int32.MaxValue))
				{
					weights[startX + 1][startY] =
						newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					results.Add(SetWeightToNeightbourForAssult(startX + 1, startY, 0));
				}
			}
			//up
			if (startY < _startParams.y_cells_count - 1 && allowedTypesTypes.Contains(battleField[startX][startY]) && forbiddenWay != 2)
			{
				var newWeight = weights[startX][startY + 1];
				if (newWeight == 0 || (newWeight > currentWeight + 1 && newWeight != Int32.MaxValue))
				{
					weights[startX][startY + 1] =
						newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					results.Add(SetWeightToNeightbourForAssult(startX, startY+1, 2));
				}
			}
			//down
			if (startY > 0 && allowedTypesTypes.Contains(battleField[startX][startY]) && forbiddenWay != 3)
			{
				var newWeight = weights[startX][startY - 1];
				if (newWeight == 0 || (newWeight > currentWeight + 1 && newWeight != Int32.MaxValue))
				{
					weights[startX][startY - 1] =
						newWeight == 0 ? currentWeight + 1 : Math.Min(newWeight, currentWeight + 1);
					results.Add(SetWeightToNeightbourForAssult(startX, startY - 1, 3));
				}
			}

			return results.OrderBy(r => r[2]).FirstOrDefault();
		}

		private int[] RollBackForAssult(int[] pos)
		{
			var ways = new int[1];
			if (pos == null) return new int[0];
			int[] newPos = new int[2];
			var currentWeight = weights[pos[0]][pos[1]];
			if (currentWeight == 0 || (pos[0] == _me[0] && pos[1] == _me[1])) return new int[0];
			if (currentWeight == 1)
			{
				if (pos[0] == _me[0] - 1) ways[0] = 0;
				if (pos[0] == _me[0] + 1) ways[0] = 1;
				if (pos[1] == _me[1] - 1) ways[0] = 3;
				if (pos[1] == _me[1] + 1) ways[0] = 2;
				return ways.Concat(RollBackForAssult(_me)).ToArray();
			}

			//left
			if (pos[0] > 0 && weights[pos[0] - 1][pos[1]] == currentWeight - 1)
			{
				ways[0] = 1;
				newPos = new[] { pos[0] - 1, pos[1] };
			}
			else
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
				ways[0] = 2;
				newPos = new[] { pos[0], pos[1] + 1 };
			}
			else
				//down
			if (pos[1] > 0 && weights[pos[0]][pos[1] - 1] == currentWeight - 1)
			{
				ways[0] = 3;
				newPos = new[] { pos[0], pos[1] - 1 };
			}
			return RollBackForAssult(newPos).Concat(ways).ToArray();
		}

		private int[] RollBack(int[] pos, int length, int limit, ref int value)
		{
			var ways = new int[1];
			if (pos == null) return ways;
			int[] newPos = new int[2];
			var currentWeight = weights[pos[0]][pos[1]];
			if (currentWeight == 0) return new int[0];
			var forbiddenTypes = new int[]
			{
				(int) CellTypes.MyTerritory,
				(int) CellTypes.Opponent,
				(int) CellTypes.MyTail
			};

			//left
			if (pos[0] > 0 && weights[pos[0] - 1][pos[1]] == currentWeight-1 && !forbiddenTypes.Contains(battleField[pos[0] - 1][pos[1]]))
			{
				ways[0] = 1;
				newPos = new[] {pos[0] - 1, pos[1]};
			} else
			//right
			if (pos[0] < _startParams.x_cells_count - 1 && weights[pos[0] + 1][pos[1]] == currentWeight - 1 && !forbiddenTypes.Contains(battleField[pos[0] + 1][pos[1]]))
			{
				ways[0] = 0;
				newPos = new[] { pos[0] + 1, pos[1] };
			}
			else
				//up
			if (pos[1] < _startParams.y_cells_count - 1 && weights[pos[0]][pos[1] + 1] == currentWeight - 1 && !forbiddenTypes.Contains(battleField[pos[0]][pos[1]+1]))
			{
				ways[0] = 3;
				newPos = new[] { pos[0], pos[1] + 1 };
			}
			else
			//down
			if (pos[1] > 0 && weights[pos[0]][pos[1] - 1] == currentWeight - 1 && !forbiddenTypes.Contains(battleField[pos[0]][pos[1]-1]))
			{
				ways[0] = 2;
				newPos = new[] { pos[0], pos[1] - 1 };
			}

			value += GetPositionValue(pos, length, limit);
			return RollBack(newPos, length - 1, limit, ref value).Concat(ways).ToArray();
		}

		public int[] BuildWeightsAndReturnWay(bool isHunt)
		{
			var maxLength = FindDeathLimit();
			int[] selectedPathEnd = { };
			weights = new int[_startParams.x_cells_count][];
			for (int i = 0; i < _startParams.x_cells_count; i++)
			{
				weights[i] = new int[_startParams.y_cells_count];
			}

			var path = new int[] { };
			var complanarWay = DirectionHelper.GetComplanarWay(_currentTickParam.players["i"].direction);
			if (isHunt)
			{
				foreach (var line in _currentTickParam.players["i"].lines)
				{
					weights[line[0]/_startParams.width][line[1] / _startParams.width] = Int32.MaxValue;
				}
				weights[_me[0]][_me[1]] = 0;
				SetWeightToNeightbourForHunt(_me[0], _me[1], maxLength + 10, complanarWay);
				var goodTerritories = _currentTickParam.players["i"].territory;
				goodTerritories = goodTerritories
					.Where(t => weights[t[0] / _startParams.width][t[1] / _startParams.width] > 0)
					.Select(t => new[]
					{
						t[0] / _startParams.width,
						t[1] / _startParams.width,
						weights[t[0] / _startParams.width][t[1] / _startParams.width]
					});
				List<Tuple<int[],int, int>> tupls = new List<Tuple<int[], int, int>>();
				foreach (var gt in goodTerritories)
				{
					var value = 0;
					var track = RollBack(gt, gt[2], maxLength, ref value);
					var turns = CalcTurns(track);
					tupls.Add(new Tuple<int[], int, int>(track,turns,value - turns));
				}

				if (maxLength <= 3 || tupls.Count(t => t.Item1.Length < maxLength) == 0)
				{
					tupls = tupls.OrderBy(t => t.Item1.Length).ToList();
				}
				else
				{
					tupls = tupls.Where(t => t.Item1.Length < maxLength).OrderByDescending(t => t.Item3).ToList();
				}

				path = tupls.FirstOrDefault().Item1;
			}
			else
			{
				selectedPathEnd = SetWeightToNeightbourForAssult(_me[0], _me[1],
					complanarWay);
				path =  RollBackForAssult(selectedPathEnd);
			}

			return path;
		}

		private int CalcTurns(int[] track)
		{
			var turns = 0;
			if (track.Length == 1)
			{
				return 0;
			}

			for (int i = 1; i < track.Length; i++)
			{
				if (track[i] != track[i - 1])
				{
					turns++;
				}
			}

			var myTail = _currentTickParam.players["i"].lines.ToArray();
			if(myTail.Count() > 2)
			{
				for (int i = 2; i < myTail.Length; i++)
				{
					if (
						(myTail[i][0] - myTail[i - 1][0] == myTail[i - 1][0] - myTail[i - 2][0]) ||
					    (myTail[i][1] - myTail[i - 1][1] == myTail[i - 1][1] - myTail[i - 2][1])
					   )
					{
						turns++;
					}
				}
			}

			if (DirectionHelper.Way(_currentTickParam.players["i"].direction) != track[0])
			{
				turns++;
			}
			return turns;
		}

		private int CalcPathValue(int[] track)
		{
			var turns = 0;
			if (track.Length == 2)
			{
				return 0;
			}

			for (int i = 2; i < track.Length; i++)
			{
				if (track[i] != track[i - 1])
				{
					turns++;
				}
			}
			return turns;
		}

		private int GetPositionValue(int[] pos, int extraTailLength, int limit)
		{
			var cellType = battleField[pos[0]][pos[1]];
			switch (cellType)
			{
				case 1:
				{
					var width = _startParams.width;
					var opponent = _currentTickParam.players.Where(p => p.Key != "i").Select(p =>
					{
						var distance = Math.Abs(p.Value.position[0] / width - pos[0]);
						distance += Math.Abs(p.Value.position[1] / width - pos[1]);
						distance += p.Value.lines.Count();
						return new Tuple<Player, int>(p.Value, distance);
					}).Where(t => t.Item2 < limit/2+1).OrderBy(t => t.Item2).FirstOrDefault();
					if (opponent != null && opponent.Item1.lines.Count() < extraTailLength + weights[pos[0]][pos[1]])
					{
						return -1000;
					}
					return 5;
				}
				case 0:
				{
					var width = _startParams.width;
					var opponent = _currentTickParam.players.Where(p => p.Key != "i").Select(p =>
						{
							var distance = Math.Abs(p.Value.position[0]/width - pos[0]);
							distance+= Math.Abs(p.Value.position[1] / width - pos[1]);
							distance += p.Value.lines.Count();
							return new Tuple<Player, int>(p.Value, distance);
						}).Where(t => t.Item2 < limit/2+1).OrderBy(t => t.Item2).FirstOrDefault();
					if (opponent != null && opponent.Item1.lines.Count() < extraTailLength + weights[pos[0]][pos[1]])
					{
						return -1000;
					}
					return 1;
				}
				case 2:
				{
					var width = _startParams.width;
					var opponent = _currentTickParam.players.FirstOrDefault(p =>
						p.Value.lines.Any(l => l[0] / width == pos[0] && l[1] / width == pos[1]));
					if (_currentTickParam.players["i"].lines.Count() + extraTailLength < opponent.Value.lines.Count())
					{
						return 50;
					}
					return Int32.MinValue;
				}
				case -1:
				{
					var isOpponentTerritory = false;
					foreach (var playerKVP in _currentTickParam.players)
					{
						isOpponentTerritory = isOpponentTerritory || (
	                      playerKVP.Value.territory.Any(t =>
	                      t[0] / _startParams.width == pos[0] &&
	                      t[1] / _startParams.width == pos[1]) ||
						  (playerKVP.Value.lines.Count() < extraTailLength + weights[pos[0]][pos[1]])
						);
					}

					if (isOpponentTerritory) return -1000;

					return 50;
				};
				default: return 0;
			}
		}
	}
}
