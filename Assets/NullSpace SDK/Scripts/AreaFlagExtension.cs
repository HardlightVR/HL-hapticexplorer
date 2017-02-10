namespace NullSpace.SDK
{
	public static class AreaFlagExtensions
	{
		//		//public static AreaFlag ConvertFlag(string flagName)
		//		//{
		//		//	return (AreaFlag)System.Enum.Parse(typeof(AreaFlag), flagName, true); ;
		//		//}
		//		//public static bool HasFlag(int baseFlag, int flag)
		//		//{
		//		//	if ((baseFlag & flag) == flag)
		//		//	{
		//		//		return true;
		//		//	}
		//		//	return false;
		//		//}
		public static bool HasFlag(this AreaFlag baseFlag, int flag)
		{
			if (((int)baseFlag & (flag)) == flag)
			{
				return true;
			}
			return false;
		}
		public static bool HasFlag(this AreaFlag baseFlag, AreaFlag checkFlag)
		{
			return HasFlag(baseFlag, (int)checkFlag);
		}
		//public static bool HasFlag(this AreaFlag baseFlag, AreaFlag flag)
		//{
		//	if ((int)baseFlag & (int)flag) > 0)
		//		return true;
		//	if (((int)baseFlag & (int)flag)) == 1)
		//	{
		//		return true;
		//	}
		//	return HasFlag(baseFlag, flag);
		//}

		//		public static string CleanString(this AreaFlag baseFlag)
		//		{
		//			//			Remove tiny faces from the enum, they have no place here.
		//			string cleanedString = baseFlag.ToString().Replace('_', ' ');

		//			return cleanedString;
		//		}
	}
}