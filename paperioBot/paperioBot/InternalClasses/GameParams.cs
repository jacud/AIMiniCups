namespace paperioBot.InternalClasses
{
	internal class GameParams<T> where T: IWorldParam
	{
		public string type { get; set; }

		public T @params { get; set; }
	}
}
