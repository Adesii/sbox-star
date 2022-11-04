namespace Star
{
	public struct Vector3Int
	{
		public int x;
		public int y;
		public int z;

		public Vector3Int( int x, int y, int z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public Vector3Int( Vector3Int v )
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
		}
		public Vector3Int( float x, float y, float z )
		{
			this.x = (int)x;
			this.y = (int)y;
			this.z = (int)z;
		}

		public static implicit operator Vector3( Vector3Int value )
		{
			return new Vector3( value.x, value.y, value.z );
		}
		public static implicit operator Vector3Int( Vector3 value )
		{
			return new Vector3Int( value.x, value.y, value.z );
		}
		public static implicit operator Vector3Int( int value )
		{
			return new Vector3Int( value, value, value );
		}

		public override string ToString()
		{
			return $"[{x},{y},{z}]";
		}

		public static Vector3Int operator +( Vector3Int c1, Vector3Int c2 )
		{
			return new Vector3Int( c1.x + c2.x, c1.y + c2.y, c1.z + c2.z );
		}

		public static Vector3Int operator *( Vector3Int c1, int f )
		{
			return new Vector3Int( c1.x * f, c1.y * f, c1.z * f );
		}

	}
}
