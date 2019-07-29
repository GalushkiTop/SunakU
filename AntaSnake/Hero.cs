using System;
using System.Collections.Generic;
//using System.Collections;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Drawing;
using PluginInterface;
using System.Windows.Forms;
using System.Threading;

namespace AntaSnake
{
	public class PathNode
	{
		// Координаты точки на карте.
		public Point Position { get; set; }
		// Длина пути от старта (G).
		public int PathLengthFromStart { get; set; }
		// Точка, из которой пришли в эту точку.
		public PathNode CameFrom { get; set; }
		// Примерное расстояние до цели (H).
		public int HeuristicEstimatePathLength { get; set; }
		// Ожидаемое полное расстояние до цели (F).
		public int EstimateFullPathLength
		{
			get
			{
				return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
			}
		}
	}


	public class Hero : BaseSnake, ISmartSnake
	{
		
		private DateTime dt = DateTime.Now;
		private static Random rnd = new Random();

		public override void Startup(Size size, List<Point> stones)
		{
			bastardFood = new HashSet<Point>();
			Name = "HeroSnake";
			Color = Color.Aqua;
			stone = stones;
		}
		
		


		protected override  Point FindGoal(Point Position, List<Point> possibleGoal)// ЕДА - НЕ КАМНИ
		{
			//int min = GetHeuristicPathLength(Position, food[0]);
			int min = int.MaxValue;
			int ind = -1;
			for (int a = 0; a < possibleGoal.Count; a++)
			{
				if (bastardFood.Contains(possibleGoal[a]) || possibleGoal[a].Equals(Position))
					continue;
				int buf = GetHeuristicPathLength(Position, possibleGoal[a]);
				if (min > buf) {
					min = buf;
					ind = a;
				}
			}
			if (ind == -1)
			{
				MessageBox.Show("ЖОПА");
				Thread.Sleep(2000);
				return possibleGoal[0];
			}
			return possibleGoal[ind];
		}

		private static List<Point> GetPathForNode(PathNode pathNode)
		{
			var result = new List<Point>();
			var currentNode = pathNode;
			while (currentNode != null)
			{
				result.Add(currentNode.Position);
				currentNode = currentNode.CameFrom;
			}
			result.Reverse();
			return result;
		}

		private  HashSet<PathNode> GetNeighbours(PathNode pathNode, Point goal, 
			List<Point> stones, List<Point> dead, List<Point> tail)
		{
			var result = new HashSet<PathNode>();

			// Соседними точками являются соседние по стороне клетки.
			Point[] neighbourPoints = new Point[4];
			neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
			neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
			neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
			neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

			foreach (var point in neighbourPoints)
			{
				// Проверяем, что по клетке можно ходить.
				if ( stones.Contains(point) || dead.Contains(point) || tail.Contains(point))
					continue;
				// Заполняем данные для точки маршрута.
				var neighbourNode = new PathNode()
				{
					Position = point,
					CameFrom = pathNode,
					PathLengthFromStart = pathNode.PathLengthFromStart +
										  GetDistanceBetweenNeighbours(),
					HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
				};
				result.Add(neighbourNode);
			}
			return result;
		}



		private static int GetDistanceBetweenNeighbours()
		{
			return 1;
		}



		protected override List<Point> FindPath(Point start, Point goal, List<Point> stones, 
			List<Point> dead, List<Point> tail, bool Reverse)
		{	
			HashSet<PathNode> closedSet = new HashSet<PathNode>();
			HashSet<PathNode> openSet = new HashSet<PathNode>();
			PathNode startNode = new PathNode()
			{
				Position = start,
				CameFrom = null,
				PathLengthFromStart = 0,
				HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
			};
			openSet.Add(startNode);
			while (openSet.Count > 0)
			{
				// Шаг 3.
				var currentNode = openSet.OrderBy(node =>
				  node.EstimateFullPathLength).First();
				// Шаг 4.
				if (currentNode.Position == goal)
					return GetPathForNode(currentNode);
				// Шаг 5.
				openSet.Remove(currentNode);
				closedSet.Add(currentNode);
				// Шаг 6.
				foreach (var neighbourNode in GetNeighbours(currentNode, goal, stones, dead, tail))
				{
					// Шаг 7.
					if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
						continue;
					var openNode = openSet.FirstOrDefault(node =>
					  node.Position == neighbourNode.Position);
					// Шаг 8.
					if (openNode == null)
						openSet.Add(neighbourNode);
					else
					  if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
					{
						// Шаг 9.
						openNode.CameFrom = currentNode;
						openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
					}
				}
			}
			// Шаг 10.
			return null;
		}

		protected override Move FindDirection(Point snake, Point goal)
		{
			sbyte x, y;
			x = Convert.ToSByte(snake.X - goal.X);
			y = Convert.ToSByte(snake.Y - goal.Y);
			if (x > 0)
				return (Move)4;
			else if (x < 0)
				return (Move)2;
			if (y < 0)
				return (Move)3;
			else if (y > 0)
				return (Move)1;
			return 0;
		}

		public override void Update(Snake snake, List<Snake> enemies, List<Point> food, List<Point> dead)
		{
			foreach (var enemy in enemies)
			{
				//food = food.Concat(enemy.Tail).ToList();
				if (enemy.Tail.Count < snake.Tail.Count)
					food = food.Concat(enemy.Tail).ToList();
			}

			base.Update(snake, enemies, food, dead);
		}
	}
}
