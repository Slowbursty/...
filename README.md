# ...
Инструкция по запуску приложения
1.	В файле config.txt, расположенном в папке с приложением, необходимо в первой строке указать полный путь к файлу access.log (например, C:\WebServers\usr\local\apache\logs\access1.log), а во второй строке указать строку подключения к базе данных (например, server=localhost;user=root;database=logs;password=password1;), где вместо localhost необходимо ввести адрес сервера MySQL, вместо root – имя пользователя MySQL, вместо logs – название базы данных, если оно отличается от предложенного, вместо password1 – пароль для входа на сервер MySQL.
2.	Создать или импортировать на сервер MySQL базу данных.
Для создания базы данных можно в командной строке mysql.exe ввести следующие команды:

CREATE DATABASE logs;
use logs;
CREATE TABLE logs (
Id INT PRIMARY KEY AUTO_INCREMENT,
Ip VARCHAR(15),
Date DATE,
Information VARCHAR(250)
);

CREATE TABLE users (
login VARCHAR(50) PRIMARY KEY,
password VARCHAR(50),
name VARCHAR(70)
);


3.	Если в базе данных отсутствуют пользователи, то внести их в таблицу users.
Добавить пользователя в таблицу из mysql.exe можно с помощью следующей команды:

use logs;
INSERT INTO users (login, password, name) VALUES ('user1', 'password1', 'name1');

4.	Запустить приложение.
