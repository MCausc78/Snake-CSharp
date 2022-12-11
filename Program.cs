using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
	enum Arrow
	{
		Up,
		Left,
		Down,
		Right,
		None,
		Quit,
	}
	class Segment
	{
		public static int AreaX = 30;
		public static int AreaZ = 15;
		public static readonly Segment NoneSegment = new();
		public static readonly Segment MaxSegment = new(AreaX, AreaZ);
		public int X { get; set; }
		public int Z { get; set; }
		public Segment()
		{
			this.X = 0;
			this.Z = 0;
		}
		public Segment(int x)
		{
			this.X = x;
		}
		public Segment(int x, int z)
		{
			this.X = x;
			this.Z = z;
		}
		public Segment(Segment s)
		{
			this.X = s.X;
			this.Z = s.Z;
		}
		public static bool operator==(Segment a, Segment b)
		{
			return a.X == b.X && a.Z == b.Z;
		}
		public static bool operator!=(Segment a, Segment b)
		{
			return !(a == b);
		}
		public static bool operator<(Segment a, Segment b)
		{
			return a.X < b.X && a.Z < b.Z;
		}
		public static bool operator<=(Segment a, Segment b)
		{
			return a.X <= b.X && a.Z <= b.Z;
		}
		public static bool operator>(Segment a, Segment b)
		{
			return a.X > b.X && a.Z > b.Z;
		}
		public static bool operator>=(Segment a, Segment b)
		{
			return a.X >= b.X && a.Z >= b.Z;
		}
		public static implicit operator bool(Segment a)
		{
			return !((a.X < 0 || a.Z < 0) || (a.X > AreaX || a.Z > AreaZ));
		}
		public static bool operator!(Segment a)
		{
			return !(bool)a;
		}
		public static Segment operator%(Segment a, Segment b)
		{
			return new Segment(a.X % b.X, a.Z % b.Z);
		}
		public static Segment operator*(Segment a, Segment b)
		{
			return new Segment(a.X * b.X, a.Z * b.Z);
		}
		public static Segment operator-(Segment a, Segment b)
		{
			return new Segment(a.X - b.X, a.Z - b.Z);
		}
		public static Segment operator+(Segment a, Segment b)
		{
			return new Segment(a.X + b.X, a.Z + b.Z);
		}
		public override bool Equals(object? a)
		{
			if (a == null || a.GetType() != this.GetType())
			{
				return false;
			}
			Segment b = (Segment) a;
			return this.Equals(b);
		}
		public bool Equals(Segment b)
		{
			return this == b;
		}
		public override int GetHashCode()
		{
			return this.X << 32 | this.Z;
		}
		public override string ToString()
		{
			return $"Segment{{X={X},Z={Z}}}";
		}
		public Segment Apply(Arrow arrow)
		{
			return arrow switch
			{
				Arrow.Up			=> this - new Segment(0, 1),
				Arrow.Left			=> this - new Segment(1, 0),
				Arrow.Down			=> this + new Segment(0, 1),
				Arrow.Right			=> this + new Segment(1, 0),
				Arrow.None			=> new Segment(this),
				_					=> throw new ArgumentException(message: "unknown arrow"),
			};
		}
	}
	class Program
	{
		static Arrow ConvertKey(ConsoleKey ck)
		{
			return ck switch
			{
				ConsoleKey.UpArrow or ConsoleKey.W		=> Arrow.Up,
				ConsoleKey.LeftArrow or ConsoleKey.A	=> Arrow.Left,
				ConsoleKey.DownArrow or ConsoleKey.S	=> Arrow.Down,
				ConsoleKey.RightArrow or ConsoleKey.D	=> Arrow.Right,
				ConsoleKey.Escape or ConsoleKey.Q		=> Arrow.Quit,
				_										=> Arrow.None,
			};
		}
		static Arrow Reverse(Arrow arrow)
		{
			return arrow switch
			{
				Arrow.Up	=> Arrow.Down,
				Arrow.Left	=> Arrow.Right,
				Arrow.Down	=> Arrow.Up,
				Arrow.Right	=> Arrow.Left,
				_			=> arrow,
			};
		}
		static int Main()
		{
			var segments = new List<Segment> {
				new Segment(2, 2),
				new Segment(3, 2),
				new Segment(4, 2),
				new Segment(5, 2)
			};
			int i;
			Random rnd = new();
			int appleX = rnd.Next(0, Segment.AreaX),
				appleZ = rnd.Next(0, Segment.AreaZ);
			Segment apple = new(appleX, appleZ);
			ConsoleKeyInfo cki;
			for(;;)
			{
				if(Console.WindowHeight < Segment.AreaX || Console.WindowWidth < Segment.AreaZ)
				{
					Console.Clear();
					Console.SetCursorPosition(0, 0);
					Console.WriteLine("size error");
					return 1;
				}
				Arrow arrow;
				switch (arrow = ConvertKey((cki = Console.ReadKey()).Key))
				{
					case Arrow.Quit:
						Console.Clear();
						goto eol;
					case Arrow.Up:
					case Arrow.Left:
					case Arrow.Down:
					case Arrow.Right:
						segments.Add(segments[^1].Apply(arrow));
						if(!segments[^1])
						{
							Console.Clear();
							Console.WriteLine("GAME OVER!!!");
							goto eol;
						}
						segments.RemoveAt(0);
						break;
				}
				if(segments.IndexOf(apple) != -1)
				{
					segments.Insert(0, segments[^1].Apply(Reverse(arrow)));
					appleX = rnd.Next(0, Segment.AreaX);
					appleZ = rnd.Next(0, Segment.AreaZ);
					apple = new Segment(appleX, appleZ);
				}
				Console.Clear();
				foreach(Segment seg in segments)
				{
					Console.SetCursorPosition(seg.X, seg.Z);
					Console.Write('@');
				}
				Console.SetCursorPosition(appleX, appleZ);
				Console.Write('A');
				Console.SetCursorPosition(0, Segment.AreaZ + 1);
				Console.Write(new String('-', Segment.AreaX + 1));
				for(i = 0; i <= Segment.AreaZ; i++)
				{
					Console.SetCursorPosition(Segment.AreaX + 1, i);
					Console.Write('|');
				}
				Console.SetCursorPosition(Segment.AreaX + 1, Segment.AreaZ + 1);
				Console.Write('+');
			}
			eol:
			Console.WriteLine("Score: {0}", segments.Count);
			return 0;
		}
	}
}
