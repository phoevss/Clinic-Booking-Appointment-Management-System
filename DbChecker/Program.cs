using System;
using Microsoft.Data.Sqlite;

var connectionStringBuilder = new SqliteConnectionStringBuilder();
connectionStringBuilder.DataSource = @"..\booking.db";
using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
{
    connection.Open();
    var command = connection.CreateCommand();
    command.CommandText = "SELECT Id, Name FROM Services";
    using (var reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine($"Service: {reader.GetInt32(0)} - {reader.GetString(1)}");
        }
    }
}
