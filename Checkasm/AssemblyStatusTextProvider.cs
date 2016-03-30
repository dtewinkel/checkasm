using System;
namespace CheckAsm
{
	public static class AssemblyStatusTextProvider
	{
		public static string GetText(AsmData.AsmValidity status)
		{
			switch (status)
			{
				case AsmData.AsmValidity.Valid:
				{
					return "OK";
				}
				case AsmData.AsmValidity.ReferencesOnly:
				{
					return "Minor issues";
				}
				case AsmData.AsmValidity.Invalid:
				{
					return "Serious issues";
				}
				case AsmData.AsmValidity.CircularDependency:
				{
					return "Circular Dependency Detected";
				}
				case AsmData.AsmValidity.Redirected:
				{
					return "OK/Redirected";
				}
				default:
				{
					return string.Empty;
				}
			}
		}
	}
}
