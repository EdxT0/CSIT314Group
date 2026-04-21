namespace CSIT_314_Group.Data
{
    public class FavouriteRepository
    {

        private readonly DbConnectionFactory _dbConnectionFactory;

        public FavouriteRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        //public async Task<int?> GetFavourites(int fraId)
        //{

        //}
    }
}
