namespace DbMigrate.Model
{
    public interface IMigrationLoader
    {
        int MaxMigrationVersionFound { get; }
        MigrationSpecification LoadMigrationIfPresent(int version);
    }
}