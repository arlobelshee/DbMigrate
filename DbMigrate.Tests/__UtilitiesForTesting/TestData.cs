using System;
using System.Collections.Generic;
using System.Linq;
using DbMigrate.Model;
using DbMigrate.Model.Support;

namespace DbMigrate.Tests.__UtilitiesForTesting
{
    internal static class TestData
    {
        public static MigrationSpecification[] Migrations(params int[] versionNumbers)
        {
            return versionNumbers.Select(MigrationForVersion).ToArray();
        }

        public static MigrationSpecification MigrationForVersion(int n)
        {
            return new MigrationSpecification(n,
                String.Format("{0}_from_memory", n),
                String.Format("do {0}", n),
                String.Format("undo {0}", n));
        }

        public static IMigrationLoader[] ToLoaders(this IEnumerable<MigrationSpecification> migrations)
        {
            return new IMigrationLoader[] {new MigrationRepoMakeDbVersionAware(), new MigrationRepoInMemory(migrations.ToList())};
        }
    }
}