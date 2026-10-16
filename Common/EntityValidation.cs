namespace CoachDesk.Common;

public static class EntityValidation
{
	public static class User
	{
		public const int UserNameMaxLength = 25;

		public const int PasswordMinLength = 8;
		public const int PasswordMaxLength = 50;

		public const int EmailMinLength = 3;
		public const int EmailMaxLength = 320;

		public const int FullNameMaxLength = 100;
	}
}
