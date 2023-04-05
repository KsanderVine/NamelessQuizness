using System.Text;

namespace NamelessQuizness.Extensions
{
    internal static class CharExtensions
	{
		public static string Repeat(this char value, int count)
		{
			StringBuilder stringBuilder = new();

			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(value);
			}

			return stringBuilder.ToString();
		}
	}
}
