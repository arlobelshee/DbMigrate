using System;

namespace DbMigrate.Model
{
	public class ChangeGoal : IEquatable<ChangeGoal>
	{
		public ChangeGoal(int currentVersion, int? targetVersion)
		{
			CurrentVersion = currentVersion;
			TargetVersion = targetVersion;
		}

		public int CurrentVersion { get; }
		public int? TargetVersion { get; }

		public bool Equals(ChangeGoal other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.CurrentVersion == CurrentVersion && other.TargetVersion == TargetVersion;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ChangeGoal);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = CurrentVersion;
				result = (result * 397) ^ (TargetVersion ?? -1);
				return result;
			}
		}

		public static bool operator ==(ChangeGoal left, ChangeGoal right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ChangeGoal left, ChangeGoal right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return string.Format("Request to go from version {0} to version {1}.", CurrentVersion,
				(object) TargetVersion ?? "latest");
		}
	}
}