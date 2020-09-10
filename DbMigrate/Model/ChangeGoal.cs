using DbMigrate.Model.Support;
using System;

namespace DbMigrate.Model
{
	public class ChangeGoal : IEquatable<ChangeGoal>
	{
		public ChangeGoal(DatabaseVersion currentVersion, long? targetMin, long? targetMax)
		{
			CurrentVersion = currentVersion;
			TargetMin = targetMin;
			TargetMax = targetMax;
		}

		public DatabaseVersion CurrentVersion { get; }
		public long? TargetMin { get; }
		public long? TargetMax { get; }

		public bool Equals(ChangeGoal other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.CurrentVersion == CurrentVersion && other.TargetMin == TargetMin
				&& other.TargetMax == TargetMax;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ChangeGoal);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return HashCode.Combine(CurrentVersion.GetHashCode(), TargetMin, TargetMax);
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
			return $"Request to go from version {CurrentVersion} to version [{TargetMin}, {(object)TargetMax ?? "latest"}].";
		}
	}
}