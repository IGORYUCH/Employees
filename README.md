# Установка

Создайте базу данных `employees` в вашей MS SQL

Выполните все скрипты из папки `SQL`

Откройте проект в VisualStudio и отредактируйте файл `App.config`:
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
  <appSettings>
    <add key="connectionString" value="Server=ROUTER\TESTSERVER; database=employees3; UID=sa; password=1234" />
    <add key="dateOutputFormat" value="yyyy.MM.dd"/>
    <add key="dateSqlFormat" value="yyyy.dd.MM"/>
  </appSettings>
</configuration>
```

Откредактируйте параметер `connectionString` в соответствии с вашими настройками

Выполните сборку проекта
