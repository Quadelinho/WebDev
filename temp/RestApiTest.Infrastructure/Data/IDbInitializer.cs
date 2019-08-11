namespace RestApiTest.Infrastructure.Data
{
    public interface IDbInitializer
    {
        void PrepareSampleData(ForumContext context, bool skipIfDbNotEmpty);
    }
}