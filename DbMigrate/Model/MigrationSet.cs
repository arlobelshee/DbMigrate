namespace DbMigrate.Model
{
    public class MigrationSet
    {
        private readonly IDatabase _database;

        public MigrationSet(ChangePlan plan, IDatabase database, params IMigrationLoader[] loaders)
        {
            this.Plan = plan;
            this.Loaders = loaders;
            this._database = database;
        }

        public IMigrationLoader[] Loaders { get; private set; }
        public ChangePlan Plan { get; private set; }

        public void ExecuteAll()
        {
            this.Plan.ApplyTo(this._database, this.Loaders);
        }
    }
}