namespace CSIT_314_Group.Data
{
    public class UserAccountRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        public UserAccountRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

    }
}
