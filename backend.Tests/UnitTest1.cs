global using Xunit;
using CSIT_314_Group.Controllers.Auth;
using CSIT_314_Group.Controllers.ProfileController;
using CSIT_314_Group.Controllers.UserAccountControllers;
using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;



namespace backend.Tests
{
    public class UnitTest1
    {
        private readonly string _testDbPath;
        private readonly IConfiguration _config;
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly UserAccount _userAccount;
        private readonly UserProfile _userProfile;


        public UnitTest1()
        {
            _testDbPath = $"test-{Guid.NewGuid()}.db";
            _config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = $"Data Source={_testDbPath}" }).Build();
            _dbConnectionFactory = new DbConnectionFactory(_config);
            _userAccount = new UserAccount(_dbConnectionFactory);
            _userProfile = new UserProfile(_dbConnectionFactory);
        }
        [Fact]
        public async Task UserAccountCreationTest()
        {
            //clean up
            if (File.Exists("test.db"))
            {
                File.Delete("test.db");
            }

            //arrange
            await initialiseTable();
            // test userAccount data
            UserAccount testUserAccountData = new UserAccount() {
                  Name = "testname1",
                  Email= "abcde@example.com",
                  PhoneNumber =  "12354125",
                  HashedPassword = "password123!",
                  ProfileId = 1,
                  IsSuspended = false
            };
            // test userProfileData
            UserProfile testUserProfileData = new UserProfile() { ProfileName = "testprofile", Description = "testdescription", Status = false };
            CreateUserAccountController createUserAccountController = new CreateUserAccountController(_userAccount, _userProfile);
            await _userProfile.CreateUserProfile(testUserProfileData);
            //act

            await createUserAccountController.CreateUser(testUserAccountData);


            //assert

            UserAccount result = await _userAccount.GetAllDetailsById(1);
            
            Assert.Equal(testUserAccountData.Name, result.Name);
            Assert.Equal(testUserAccountData.Email, result.Email);
            Assert.Equal(testUserAccountData.PhoneNumber, result.PhoneNumber);
            Assert.Equal(testUserAccountData.ProfileId, result.ProfileId);
            Assert.Equal(testUserAccountData.IsSuspended, result.IsSuspended);




        }


        [Fact]
        public async Task UserProfileCreationTest()
        {

            if (File.Exists("test.db"))
            {
                File.Delete("test.db");
            }
            //arrange
            await initialiseTable();

            UserProfile testUserProfileData = new UserProfile() { ProfileName = "testprofile", Description = "testdescription", Status = false };
            CreateUserProfileController updateUserProfile = new CreateUserProfileController(_userProfile);

            await updateUserProfile.CreateProfile(testUserProfileData);

            //act
            UserProfile result = await _userProfile.GetUserProfile(1);

            //assert
            Assert.Equal(testUserProfileData.ProfileName, result.ProfileName);
            Assert.Equal(testUserProfileData.Description, result.Description);
            Assert.Equal(testUserProfileData.Status, result.Status);

        }

        [Fact]
        public async Task UpdateUserAccountTest() {
            if (File.Exists("test.db"))
            {
                File.Delete("test.db");
            }

            //arrange
            await initialiseTable();
            // test userAccount data
            UserAccount testUserAccountData = new UserAccount()
            {
                Name = "testname1",
                Email = "abcde@example.com",
                PhoneNumber = "12354125",
                HashedPassword = "password123!",
                ProfileId = 1,
                IsSuspended = false
            };
            // test userProfileData
            UserProfile testUserProfileData = new UserProfile() { ProfileName = "testprofile", Description = "testdescription", Status = false };
            await _userProfile.CreateUserProfile(testUserProfileData);
            await _userAccount.CreateUser(testUserAccountData);

            UserAccount updateUser = new UserAccount() { Id=1, Email = "test@test.com", PhoneNumber = "81234567", Name = "testname2" };
            PasswordHasher<UserAccount> _hasher = new PasswordHasher<UserAccount>();
            UpdateUserAccountController updateUserAccountController = new UpdateUserAccountController(_userAccount, _userProfile, _hasher); 
            //act

            await updateUserAccountController.UpdateUserAccount(updateUser);

            UserAccount result = await _userAccount.GetAllDetailsById(1);

            //assert
            Assert.Equal(updateUser.Name, result.Name);
            Assert.Equal(updateUser.Email, result.Email);
            Assert.Equal(updateUser.PhoneNumber, result.PhoneNumber);
        }

        [Fact]
        public async Task UpdateUserProfileTest() {
            if (File.Exists("test.db"))
            {
                File.Delete("test.db");
            }

            //arrange
            await initialiseTable();
            // test userProfileData
            UserProfile testUserProfileData = new UserProfile() { ProfileName = "testprofile", Description = "testdescription", Status = false };
            await _userProfile.CreateUserProfile(testUserProfileData);

            UserProfile newUserProfile = new UserProfile() { Id=1, ProfileName = "new test", Description = "new test desc"};
            UpdateUserProfileController updateUserProfile = new UpdateUserProfileController(_userProfile);
            //act

            await updateUserProfile.UpdateUserProfile(newUserProfile);

            //assert
            UserProfile result = await _userProfile.GetUserProfile(1);

            Assert.Equal(newUserProfile.ProfileName, result.ProfileName);
            Assert.Equal(newUserProfile.Description, result.Description);
        }

        
        private async Task initialiseTable()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync();
            try
            {

                string createUserProfileTableQuery = @"CREATE TABLE IF NOT EXISTS UserProfile(
                                            Id INTEGER PRIMARY KEY,
                                            ProfileName TEXT NOT NULL UNIQUE,
                                            Description TEXT,
                                            Status BOOL NOT NULL
                                            )";
                using (var createUserProfileTableQueryCommand = new SqliteCommand(createUserProfileTableQuery, connection, transaction))
                {
                    createUserProfileTableQueryCommand.ExecuteNonQuery();
                }

                string createUserAccountTableQuery = @"CREATE TABLE IF NOT EXISTS UserAccount(
                                                     Id INTEGER PRIMARY KEY,
                                                     Name TEXT NOT NULL,
                                                     PhoneNumber TEXT NOT NULL UNIQUE,
                                                     Email TEXT NOT NULL UNIQUE,
                                                     IsSuspended BOOL NOT NULL,
                                                     HashedPassword TEXT NOT NULL,
                                                     ProfileId INTEGER NOT NULL,
                                                     FOREIGN KEY (ProfileId) References UserProfile(Id)
                                                    )";
                using (var createUserAccountTableQueryCommand = new SqliteCommand(createUserAccountTableQuery, connection, transaction))
                {
                    createUserAccountTableQueryCommand.ExecuteNonQuery();
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}
