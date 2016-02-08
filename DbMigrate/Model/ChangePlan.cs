using System;
using System.Collections.Generic;
using System.Linq;
using DbMigrate.Model.Support;
using DbMigrate.UI;

namespace DbMigrate.Model
{
    public class ChangePlan : IEquatable<ChangePlan>
    {
        public ChangePlan(Do operation, IEnumerable<int> migrationsInOrder)
        {
            this.Operation = operation;
            this.MigrationsInOrder = migrationsInOrder;
        }

        public Do Operation { get; private set; }
        public IEnumerable<int> MigrationsInOrder { get; private set; }

        public bool Equals(ChangePlan other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Operation, this.Operation) && other.MigrationsInOrder.SequenceEqual(this.MigrationsInOrder);
        }

        public void ApplyTo(IDatabase database, IMigrationLoader[] migrationLoaders)
        {
            if (this.MigrationsInOrder.Count() == 0) return;
            foreach (var version in this.MigrationsInOrder)
            {
                var migration = LoadMigrationForVersion(version, migrationLoaders);
                Require.Not(migration == null, 1, UserMessage.ErrorMissingMigration, this.Operation, version);
                User.Notify(this.Operation.UserMessageFor(version));
                this.Operation.Execute(database, migration);
            }
            this.Operation.SetResultingVersion(database, this.MigrationsInOrder.Last());
            database.Commit();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ChangePlan);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Operation.GetHashCode()*397) ^ this.MigrationsInOrder.GetHashCode();
            }
        }

        public static bool operator ==(ChangePlan left, ChangePlan right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChangePlan left, ChangePlan right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0} the migrations {1}.", this.Operation, this.MigrationsInOrder.StringJoin(", "));
        }

        private static MigrationSpecification LoadMigrationForVersion(int version,
            IEnumerable<IMigrationLoader> migrationLoaders)
        {
            return migrationLoaders
                .Select(loader => loader.LoadMigrationIfPresent(version))
                .FirstOrDefault(migration => migration != null);
        }
    }
}