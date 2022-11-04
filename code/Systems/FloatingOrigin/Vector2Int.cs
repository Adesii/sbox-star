namespace Star
{
	public struct Vector2Int
	{
		public int x;
		public int y;

		public Vector2Int( int x, int y )
		{
			this.x = x;
			this.y = y;
		}
		public Vector2Int( Vector2Int v )
		{
			this.x = v.x;
			this.y = v.y;
		}
		public Vector2Int( float x, float y )
		{
			this.x = (int)x;
			this.y = (int)y;
		}

		public static implicit operator Vector2( Vector2Int value )
		{
			return new Vector2( value.x, value.y );
		}
		public static implicit operator Vector2Int( Vector2 value )
		{
			return new Vector2Int( value.x, value.y );
		}
		public static implicit operator Vector2Int( int value )
		{
			return new Vector2Int( value, value );
		}

		public override string ToString()
		{
			return $"[{x},{y}]";
		}

		public static Vector2Int operator +( Vector2Int c1, Vector2Int c2 )
		{
			return new Vector2Int( c1.x + c2.x, c1.y + c2.y );
		}

		public static Vector2Int operator *( Vector2Int c1, int f )
		{
			return new Vector2Int( c1.x * f, c1.y * f );
		}

	}
}
