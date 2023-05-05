using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.DataAccess
{
    public class MongoDataAccess
    {

        private IMongoDatabase _db;


        public MongoDataAccess(IMongoDatabase db)
        {
            _db = db;
        }

        public MongoDataAccess(string database)
        {
            var client = new MongoClient();
            _db = client.GetDatabase(database);
        }
    }
}
