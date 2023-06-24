using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApacheLogs
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MySqlConnection conn;
            string sql;
            MySqlCommand command;
            MySqlDataReader reader;
            DateTime date = DateTime.Now;
            Console.WriteLine(date);
            string path = "config.txt";   // путь к файлу с настройками
            String pathFile = "";
            string connectionString = "";
            //Считывание конфигурации
            using (FileStream fstream2 = File.OpenRead(path))
            {
                // выделяем массив для считывания данных из файла
                byte[] buffer2 = new byte[fstream2.Length];
                // считываем данные
                await fstream2.ReadAsync(buffer2, 0, buffer2.Length);
                // декодируем байты в строку
                string textFromFile2 = Encoding.Default.GetString(buffer2);
                Regex regexF = new Regex(@".*\.log");
                MatchCollection matchesF = regexF.Matches(textFromFile2);
                pathFile = matchesF[0].Value;//файл access.log
                Regex regexD = new Regex("server.*");
                MatchCollection matchesD = regexD.Matches(textFromFile2);
                connectionString = matchesD[0].Value;//строка подключения к БД
            }

            //Авторизация
            Console.WriteLine("Введите имя пользователя:");
            String login = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            String password = Console.ReadLine();
            // создаём объект для подключения к БД
            conn = new MySqlConnection(connectionString);
            // устанавливаем соединение с БД
            conn.Open();
            // запрос
            sql = "SELECT * FROM users WHERE login = '" + login +"' AND password = '" + password + "'";
            // объект для выполнения SQL-запроса
            command = new MySqlCommand(sql, conn);
            // объект для чтения ответа сервера
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine("Добро пожаловать, " + reader[2].ToString() + "!");
                conn.Close();
                conn.Open();
                // запрос
                sql = "DELETE FROM logs;";
                // объект для выполнения SQL-запроса
                command = new MySqlCommand(sql, conn);
                // выполняем запрос
                command.ExecuteNonQuery();
                // закрываем подключение к БД
                conn.Close();

                // чтение из файла
                string textFromFile;
                using (FileStream fstream1 = File.OpenRead(pathFile))
                {
                    // выделяем массив для считывания данных из файла
                    byte[] buffer1 = new byte[fstream1.Length];
                    // считываем данные
                    await fstream1.ReadAsync(buffer1, 0, buffer1.Length);
                    // декодируем байты в строку
                    textFromFile = Encoding.Default.GetString(buffer1);
                }
                    Regex regex = new Regex(".*\n");
                    MatchCollection matches = regex.Matches(textFromFile);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            Regex regex1 = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.\d{1,3}\.\d{1,3}");
                            MatchCollection matches1 = regex1.Matches(match.Value);
                            String Ip = matches1[0].Value;
                            //foreach (Match match1 in matches1)
                            //{
                            //    Console.WriteLine("IP:" + match1.Value);
                            //}
                            Regex regex2 = new Regex(@"\d{2}/[J|F|M|A|S|O|N|D]{1}[a-z]{2}/\d{4}");
                            MatchCollection matches2 = regex2.Matches(match.Value);
                            String month = "00";
                            switch (matches2[0].Value.Substring(3, 3))
                            {
                                case "Jan":
                                    month = "01";
                                    break;
                                case "Feb":
                                    month = "02";
                                    break;
                                case "Mar":
                                    month = "03";
                                    break;
                                case "Apr":
                                    month = "04";
                                    break;
                                case "May":
                                    month = "05";
                                    break;
                                case "Jun":
                                    month = "06";
                                    break;
                                case "Jul":
                                    month = "07";
                                    break;
                                case "Aug":
                                    month = "08";
                                    break;
                                case "Sep":
                                    month = "09";
                                    break;
                                case "Oct":
                                    month = "10";
                                    break;
                                case "Nov":
                                    month = "11";
                                    break;
                                case "Dec":
                                    month = "12";
                                    break;
                                default:
                                    break;
                            }
                            String Date = matches2[0].Value.Substring(7, 4) + "-" + month + "-" + matches2[0].Value.Substring(0, 2);
                            //foreach (Match match2 in matches2)
                            //{
                            //    Console.WriteLine("Date:" + match2.Value);
                            //}
                            Regex regex3 = new Regex(@":.*");
                            MatchCollection matches3 = regex3.Matches(match.Value);
                            String Information = matches3[0].Value.Substring(1, 14) + matches3[0].Value.Substring(16);
                            //foreach (Match match3 in matches3)
                            //{
                            //    Console.WriteLine("Information:" + match3.Value);
                            //}
                            // строка подключения к базе данных
                            //connectionString = "server=localhost;user=root;database=logs;password=23021982;";
                            // объект для установления соединения с БД
                            MySqlConnection connection = new MySqlConnection(connectionString);
                            // открываем соединение
                            connection.Open();
                            // запросы
                            // запрос вставки данных
                            string query = "INSERT INTO logs (Ip, Date, Information) VALUES ('" + Ip + "', '" + Date + "', '" + Information + "')";
                            // запрос обновления данных
                            //string query2 = "UPDATE men SET age = 22 WHERE id = 4";
                            // запрос удаления данных
                            //string query3 = "DELETE FROM men WHERE id = 4";
                            // объект для выполнения SQL-запроса
                            MySqlCommand command1 = new MySqlCommand(query, connection);
                            // выполняем запрос
                            command1.ExecuteNonQuery();
                            // закрываем подключение к БД
                            connection.Close();
                        }

                    }
                    else
                    {
                        Console.WriteLine("Совпадений не найдено");
                    }
                    Boolean flag = true;
                    DateTime dateO = date;
                    while (flag)
                    {
                        DateTime dateN = DateTime.Now;
                        //Console.WriteLine(dateN.Subtract(dateO));
                        if (dateN > dateO.AddMinutes(1))
                        {
                            Console.WriteLine("База данных обновлена");

                            // объект для установления соединения с БД
                            MySqlConnection connectionD = new MySqlConnection(connectionString);
                            // открываем соединение
                            connectionD.Open();
                            // запросы
                            // запрос вставки данных
                            string queryD = "DELETE FROM logs;";
                            // запрос обновления данных
                            //string query2 = "UPDATE men SET age = 22 WHERE id = 4";
                            // запрос удаления данных
                            //string query3 = "DELETE FROM men WHERE id = 4";
                            // объект для выполнения SQL-запроса
                            MySqlCommand commandD = new MySqlCommand(queryD, connectionD);
                            // выполняем запрос
                            commandD.ExecuteNonQuery();
                            // закрываем подключение к БД
                            connectionD.Close();
                            using (FileStream fstreamU = File.OpenRead(pathFile))
                            {
                                // выделяем массив для считывания данных из файла
                                byte[] bufferU = new byte[fstreamU.Length];
                                // считываем данные
                                await fstreamU.ReadAsync(bufferU, 0, bufferU.Length);
                                // декодируем байты в строку
                                string textFromFileU = Encoding.Default.GetString(bufferU);
                                Regex regexU = new Regex(".*\n");
                                MatchCollection matchesU = regex.Matches(textFromFileU);
                                if (matchesU.Count > 0)
                                {
                                    foreach (Match match in matchesU)
                                    {
                                        Regex regex1 = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.\d{1,3}\.\d{1,3}");
                                        MatchCollection matches1 = regex1.Matches(match.Value);
                                        String Ip = matches1[0].Value;
                                        //foreach (Match match1 in matches1)
                                        //{
                                        //    Console.WriteLine("IP:" + match1.Value);
                                        //}
                                        Regex regex2 = new Regex(@"\d{2}/[J|F|M|A|S|O|N|D]{1}[a-z]{2}/\d{4}");
                                        MatchCollection matches2 = regex2.Matches(match.Value);
                                        String month = "00";
                                        switch (matches2[0].Value.Substring(3, 3))
                                        {
                                            case "Jan":
                                                month = "01";
                                                break;
                                            case "Feb":
                                                month = "02";
                                                break;
                                            case "Mar":
                                                month = "03";
                                                break;
                                            case "Apr":
                                                month = "04";
                                                break;
                                            case "May":
                                                month = "05";
                                                break;
                                            case "Jun":
                                                month = "06";
                                                break;
                                            case "Jul":
                                                month = "07";
                                                break;
                                            case "Aug":
                                                month = "08";
                                                break;
                                            case "Sep":
                                                month = "09";
                                                break;
                                            case "Oct":
                                                month = "10";
                                                break;
                                            case "Nov":
                                                month = "11";
                                                break;
                                            case "Dec":
                                                month = "12";
                                                break;
                                            default:
                                                break;
                                        }
                                        String Date = matches2[0].Value.Substring(7, 4) + "-" + month + "-" + matches2[0].Value.Substring(0, 2);
                                        //foreach (Match match2 in matches2)
                                        //{
                                        //    Console.WriteLine("Date:" + match2.Value);
                                        //}
                                        Regex regex3 = new Regex(@":.*");
                                        MatchCollection matches3 = regex3.Matches(match.Value);
                                        String Information = matches3[0].Value.Substring(1, 14) + matches3[0].Value.Substring(16);
                                        //foreach (Match match3 in matches3)
                                        //{
                                        //    Console.WriteLine("Information:" + match3.Value);
                                        //}
                                        // строка подключения к базе данных
                                        //connectionString = "server=localhost;user=root;database=logs;password=23021982;";
                                        // объект для установления соединения с БД
                                        MySqlConnection connection = new MySqlConnection(connectionString);
                                        // открываем соединение
                                        connection.Open();
                                        // запросы
                                        // запрос вставки данных
                                        string query = "INSERT INTO logs (Ip, Date, Information) VALUES ('" + Ip + "', '" + Date + "', '" + Information + "')";
                                        // запрос обновления данных
                                        //string query2 = "UPDATE men SET age = 22 WHERE id = 4";
                                        // запрос удаления данных
                                        //string query3 = "DELETE FROM men WHERE id = 4";
                                        // объект для выполнения SQL-запроса
                                        MySqlCommand commandU = new MySqlCommand(query, connection);
                                        // выполняем запрос
                                        commandU.ExecuteNonQuery();
                                        // закрываем подключение к БД
                                        connection.Close();
                                    }

                                }
                                else
                                {
                                    Console.WriteLine("Совпадений не найдено");
                                }
                            }



                            dateO = dateN;
                        }
                        Console.WriteLine("Список команд:\n" +
                            "/update - обновить базу данных\n" +
                            "/all - вывести все записи из базы данных,\n" +
                            "/ip - вывести количество обращений с каждого ip,\n" +
                            "/date - вывести количество обращений по датам,\n" +
                            "/period - вывести количество обращений за период,\n" +
                            "/exit - выход.\n");
                        Console.WriteLine("Введите команду:");
                        String userCommand = Console.ReadLine();
                        switch (userCommand)
                        {
                            case "/update":
                                // объект для установления соединения с БД
                                MySqlConnection connectionD = new MySqlConnection(connectionString);
                                // открываем соединение
                                connectionD.Open();
                                // запросы
                                // запрос вставки данных
                                string queryD = "DELETE FROM logs;";
                                // запрос обновления данных
                                //string query2 = "UPDATE men SET age = 22 WHERE id = 4";
                                // запрос удаления данных
                                //string query3 = "DELETE FROM men WHERE id = 4";
                                // объект для выполнения SQL-запроса
                                MySqlCommand commandD = new MySqlCommand(queryD, connectionD);
                                // выполняем запрос
                                commandD.ExecuteNonQuery();
                                // закрываем подключение к БД
                                connectionD.Close();
                                using (FileStream fstreamU = File.OpenRead(pathFile))
                                {
                                    // выделяем массив для считывания данных из файла
                                    byte[] bufferU = new byte[fstreamU.Length];
                                    // считываем данные
                                    await fstreamU.ReadAsync(bufferU, 0, bufferU.Length);
                                    // декодируем байты в строку
                                    string textFromFileU = Encoding.Default.GetString(bufferU);
                                    Regex regexU = new Regex(".*\n");
                                    MatchCollection matchesU = regex.Matches(textFromFileU);
                                    if (matchesU.Count > 0)
                                    {
                                        foreach (Match match in matchesU)
                                        {
                                            Regex regex1 = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.\d{1,3}\.\d{1,3}");
                                            MatchCollection matches1 = regex1.Matches(match.Value);
                                            String Ip = matches1[0].Value;
                                            //foreach (Match match1 in matches1)
                                            //{
                                            //    Console.WriteLine("IP:" + match1.Value);
                                            //}
                                            Regex regex2 = new Regex(@"\d{2}/[J|F|M|A|S|O|N|D]{1}[a-z]{2}/\d{4}");
                                            MatchCollection matches2 = regex2.Matches(match.Value);
                                            String month = "00";
                                            switch (matches2[0].Value.Substring(3, 3))
                                            {
                                                case "Jan":
                                                    month = "01";
                                                    break;
                                                case "Feb":
                                                    month = "02";
                                                    break;
                                                case "Mar":
                                                    month = "03";
                                                    break;
                                                case "Apr":
                                                    month = "04";
                                                    break;
                                                case "May":
                                                    month = "05";
                                                    break;
                                                case "Jun":
                                                    month = "06";
                                                    break;
                                                case "Jul":
                                                    month = "07";
                                                    break;
                                                case "Aug":
                                                    month = "08";
                                                    break;
                                                case "Sep":
                                                    month = "09";
                                                    break;
                                                case "Oct":
                                                    month = "10";
                                                    break;
                                                case "Nov":
                                                    month = "11";
                                                    break;
                                                case "Dec":
                                                    month = "12";
                                                    break;
                                                default:
                                                    break;
                                            }
                                            String Date = matches2[0].Value.Substring(7, 4) + "-" + month + "-" + matches2[0].Value.Substring(0, 2);
                                            //foreach (Match match2 in matches2)
                                            //{
                                            //    Console.WriteLine("Date:" + match2.Value);
                                            //}
                                            Regex regex3 = new Regex(@":.*");
                                            MatchCollection matches3 = regex3.Matches(match.Value);
                                            String Information = matches3[0].Value.Substring(1, 14) + matches3[0].Value.Substring(16);
                                            //foreach (Match match3 in matches3)
                                            //{
                                            //    Console.WriteLine("Information:" + match3.Value);
                                            //}
                                            // строка подключения к базе данных
                                            //connectionString = "server=localhost;user=root;database=logs;password=23021982;";
                                            // объект для установления соединения с БД
                                            MySqlConnection connection = new MySqlConnection(connectionString);
                                            // открываем соединение
                                            connection.Open();
                                            // запросы
                                            // запрос вставки данных
                                            string query = "INSERT INTO logs (Ip, Date, Information) VALUES ('" + Ip + "', '" + Date + "', '" + Information + "')";
                                            // запрос обновления данных
                                            //string query2 = "UPDATE men SET age = 22 WHERE id = 4";
                                            // запрос удаления данных
                                            //string query3 = "DELETE FROM men WHERE id = 4";
                                            // объект для выполнения SQL-запроса
                                            MySqlCommand commandU = new MySqlCommand(query, connection);
                                            // выполняем запрос
                                            commandU.ExecuteNonQuery();
                                            // закрываем подключение к БД
                                            connection.Close();
                                        }

                                    }
                                    else
                                    {
                                        Console.WriteLine("Совпадений не найдено");
                                    }
                                }
                                break;
                            case "/all":
                                // строка подключения к БД
                                //string connStr = "server=localhost;user=root;database=logs;password=23021982;";
                                // создаём объект для подключения к БД
                                //conn = new MySqlConnection(connectionString);
                                // устанавливаем соединение с БД
                                conn.Open();
                                // запрос
                                sql = "SELECT * FROM logs";
                                // объект для выполнения SQL-запроса
                                command = new MySqlCommand(sql, conn);
                                // объект для чтения ответа сервера
                                reader = command.ExecuteReader();
                                Console.WriteLine("     IP            Дата                        Информация");
                                // читаем результат
                                while (reader.Read())
                                {
                                    // элементы массива [] - это значения столбцов из запроса SELECT
                                    Console.WriteLine(reader[1].ToString() + "      " + reader[2].ToString().Substring(0, 10) + "      " + reader[3].ToString());
                                }
                                reader.Close(); // закрываем reader
                                                // закрываем соединение с БД
                                conn.Close();
                                break;
                            case "/ip":
                                // строка подключения к БД
                                //string connStr1 = "server=localhost;user=root;database=logs;password=23021982;";
                                // создаём объект для подключения к БД
                                MySqlConnection conn1 = new MySqlConnection(connectionString);
                                // устанавливаем соединение с БД
                                conn1.Open();
                                // запрос
                                string sql1 = "SELECT Ip, COUNT(*) As Requests FROM logs GROUP BY Ip;";
                                // объект для выполнения SQL-запроса
                                MySqlCommand command1 = new MySqlCommand(sql1, conn1);
                                // объект для чтения ответа сервера
                                MySqlDataReader reader1 = command1.ExecuteReader();
                                Console.WriteLine("    IP          Количество запросов");
                                // читаем результат
                                while (reader1.Read())
                                {
                                    // элементы массива [] - это значения столбцов из запроса SELECT
                                    Console.WriteLine(reader1[0].ToString() + "       " + reader1[1].ToString());
                                }
                                reader1.Close(); // закрываем reader
                                                 // закрываем соединение с БД
                                conn1.Close();
                                break;
                            case "/date":
                                // строка подключения к БД
                                //string connStr2 = "server=localhost;user=root;database=logs;password=23021982;";
                                // создаём объект для подключения к БД
                                MySqlConnection conn2 = new MySqlConnection(connectionString);
                                // устанавливаем соединение с БД
                                conn2.Open();
                                // запрос
                                string sql2 = "SELECT Date, COUNT(*) As Requests FROM logs GROUP BY Date;";
                                // объект для выполнения SQL-запроса
                                MySqlCommand command2 = new MySqlCommand(sql2, conn2);
                                // объект для чтения ответа сервера
                                MySqlDataReader reader2 = command2.ExecuteReader();
                                Console.WriteLine("  Дата          Количество запросов");
                                // читаем результат
                                while (reader2.Read())
                                {
                                    // элементы массива [] - это значения столбцов из запроса SELECT
                                    Console.WriteLine(reader2[0].ToString().Substring(0, 10) + "      " + reader2[1].ToString());
                                }
                                reader2.Close(); // закрываем reader
                                                 // закрываем соединение с БД
                                conn2.Close();
                                break;
                            case "/period":
                                Console.WriteLine("Введите начало периода в формате ГГГГ-ММ-ДД:");
                                String startDate = Console.ReadLine();
                                Regex regexST = new Regex(@"^\d{4}-\d{2}-\d{2}$");
                                MatchCollection matchesST = regexST.Matches(startDate);
                                if (matchesST.Count > 0)
                            {
                                //C:\WebServers\usr\local\apache\logs
                                Console.WriteLine("Введите конец периода в формате ГГГГ-ММ-ДД:");
                                String endDate = Console.ReadLine();
                                Regex regexEN = new Regex(@"^\d{4}-\d{2}-\d{2}$");
                                MatchCollection matchesEN = regexEN.Matches(endDate);
                                if (matchesEN.Count > 0)
                                {
                                    // строка подключения к БД
                                    //string connStr3 = "server=localhost;user=root;database=logs;password=23021982;";
                                    // создаём объект для подключения к БД
                                    MySqlConnection conn3 = new MySqlConnection(connectionString);
                                    // устанавливаем соединение с БД
                                    conn3.Open();
                                    // запрос
                                    string sql3 = "SELECT * FROM logs WHERE Date >= '" + startDate + "' AND Date <= '" + endDate + "';";
                                    // объект для выполнения SQL-запроса
                                    MySqlCommand command3 = new MySqlCommand(sql3, conn3);
                                    // объект для чтения ответа сервера
                                    MySqlDataReader reader3 = command3.ExecuteReader();
                                    Console.WriteLine("     IP            Дата                        Информация");
                                    // читаем результат
                                    while (reader3.Read())
                                    {
                                        // элементы массива [] - это значения столбцов из запроса SELECT
                                        Console.WriteLine(reader3[1].ToString() + "        " + reader3[2].ToString().Substring(0, 10) + "        " + reader3[3].ToString());
                                    }
                                    reader3.Close(); // закрываем reader
                                                     // закрываем соединение с БД
                                    conn3.Close();
                                }
                                else
                                {
                                    Console.WriteLine("Неправильный формат даты");
                                }
                                    
                            }
                            else
                            {
                                Console.WriteLine("Неправильный формат даты");
                            }
                                
                                break;
                            case "/exit":
                                flag = false;
                                break;
                            default:
                                Console.WriteLine("Введенное значение не соответствует ни одной из команд.");
                                break;
                        }

                    }
                    //C:\WebServers\usr\local\apache\logs
                    //Console.WriteLine(userCommand);
                    //String fileName = Console.ReadLine();

                    //Console.WriteLine($"Текст из файла:\n {textFromFile}");
                //}

            }
            else {
                Console.WriteLine("У Вас нет прав доступа к базе данных!");
            }
            
            
            reader.Close(); // закрываем reader


            Console.ReadKey();
        }
    }
}
