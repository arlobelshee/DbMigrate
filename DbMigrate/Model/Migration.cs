namespace DbMigrate.Model
{
    public class Migration
    {
        public enum Go
        {
            Up,
            Down
        }

        public Migration(Go direction, MigrationSpecification spec)
        {
            this.Direction = direction;
            this.Spec = spec;
        }

        public Go Direction { get; private set; }
        public MigrationSpecification Spec { get; private set; }
    }
}