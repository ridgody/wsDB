using Oracle.ManagedDataAccess.Client;
using wsDB.Common.DBObject.Models;

namespace wsDB.Common.DBObject.Services
{
    public class DatabaseObjectService
    {
        private readonly OracleConnection _connection;

        public DatabaseObjectService(OracleConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<DatabaseObject>> GetObjectsByNameAsync(string objectName)
        {
            const string query = @"
                SELECT owner, object_name, object_type 
                FROM all_objects 
                WHERE object_type IN ('TABLE','INDEX') 
                  AND object_name = :object_name
                ORDER BY owner, object_type";

            var objects = new List<DatabaseObject>();

            using (var command = new OracleCommand(query, _connection))
            {
                command.Parameters.Add(new OracleParameter(":object_name", objectName.ToUpper()));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    int ownerIndex = reader.GetOrdinal("OWNER");
                    int objectNameIndex = reader.GetOrdinal("OBJECT_NAME");
                    int objectTypeIndex = reader.GetOrdinal("OBJECT_TYPE");

                    while (await reader.ReadAsync())
                    {
                        objects.Add(new DatabaseObject
                        {
                            Owner = reader.GetString(ownerIndex),
                            ObjectName = reader.GetString(objectNameIndex),
                            ObjectType = reader.GetString(objectTypeIndex)
                        });
                    }
                }
            }

            return objects;
        }
    }
}