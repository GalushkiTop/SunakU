using System;
using System.Collections.Generic;
//using System.Collections;
//using System.Text;
//using System.Threading.Tasks;
using System.Drawing;
using PluginInterface;

namespace AntaSnake
{
	public abstract class BaseSnake 
	{
		public Move Direction { get; set; }
		//public List<Point> Tail { get; set; }
		public bool Reverse { get; set; }
		public string Name { get; set; }
		public Color Color { get; set; }
		public List<Point> stone;
		public HashSet<Point> bastardFood;
		public int reverseCounter = 0;
		private DateTime dt = DateTime.Now;
		private static Random rnd = new Random();



		public abstract void Startup(Size size, List<Point> stones);


		public void Startup(Size size, List<Point> stones, string name, Color color)
		{
			bastardFood = new HashSet<Point>();
			Name = name;
			Color = color;
			stone = stones;
		}


		

		protected int GetHeuristicPathLength(Point from, Point to)
		{
			return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
		}



		protected virtual Move FindDirection(Point snake, Point goal)
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

		

		public virtual void Update(Snake snake, List<Snake> enemies, List<Point> food, List<Point> dead)
		{
			Point goal = FindGoal(snake.Position, food);
			List<Point> path;
			path = FindPath(snake.Position, goal, stone, dead, snake.Tail, Reverse);

			if (path == null)
			{
				Reverse = true;
				reverseCounter += 1;

			}
			else
			{
				reverseCounter = 0;
				Point point = path[1];
				Direction = FindDirection(snake.Position, point);
			}
			if (reverseCounter > 1)
			{
				food.Remove(goal);
				bastardFood.Add(goal);
				reverseCounter = 0;
			}
		}

		protected abstract List<Point> FindPath(Point position, Point goal, List<Point> stone, List<Point> dead, List<Point> tail, bool reverse);
		protected abstract Point FindGoal(Point position, List<Point> food);
	}
}
