using System;
using System.Data;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "server=db4free.net;port=3306;database=isoevagulafzo;uid=gulafzo;password=_TZWsQ836d@_wAf;";
        MySqlConnection connection = new MySqlConnection(connectionString);

        try
        {
            connection.Open();

            string query = "SELECT Characters.Name, CharactersClass.Name AS class FROM Characters JOIN CharactersClass ON Characters.CharacterClassId  = CharactersClass.Id ";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string characterName = reader.GetString("name");
                string className = reader.GetString("class");
                Console.WriteLine("Character: {0}, Class: {1}", characterName, className);
            }

            reader.Close();
            // запрос имени и класса персонажа
            Console.WriteLine("Введите имя персонажа: ");
            string name = Console.ReadLine();
            Console.WriteLine("Введите класс персонажа: ");
            string char_class = Console.ReadLine();

            // проверка наличия класса в БД
            query = "SELECT * FROM CharactersClass WHERE Name=@ClassName";
            command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ClassName", char_class);
            MySqlDataReader classReader = command.ExecuteReader();
            if (classReader.HasRows)
            {
                classReader.Close();
                // добавление нового персонажа в таблицу Characters
                query = "INSERT INTO Characters (Name, CharacterClassId) VALUES (@Name, (SELECT Id FROM CharactersClass WHERE Name=@ClassName))";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@ClassName", char_class);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("{0} строк добавлено в таблицу Characters", rowsAffected);
            }
            else
            {
                classReader.Close();
                // добавление нового класса в таблицу CharactersClass
                query = "INSERT INTO CharactersClass (Name) VALUES (@ClassName)";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassName", char_class);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("{0} строк добавлено в таблицу CharactersClass", rowsAffected);

                // добавление нового персонажа в таблицу Characters с новым классом
                query = "INSERT INTO Characters (Name, CharacterClassId) VALUES (@Name, (SELECT MAX(Id) FROM CharactersClass))";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("{0} строк добавлено в таблицу Characters", rowsAffected);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
}

   