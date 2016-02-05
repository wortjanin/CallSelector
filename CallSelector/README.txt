В этом проекте находятся файлы, необходимые для сборки решения тестового задания.

Основные этапы инсталляции:

1. Создать базу данных (скрипты с комментариями находятся в проекте CallSelectorSqlServer) и логин call_selector / call_selector_pass,
    с назначением ему привелегий на чтение и запись базы данных db_call.
2. Пересобрать проекты CallSelectorLib, CallSelector, CallSelectorSetup, CallSelectorWeb
3. Инсталлировать сервисную службу Windows (CallSelectorSetup->Right click->Install... Ok, Ok, Complete)
4. Настроить конфигурационные файлы CallSelectorConfig.xml и CallSelectorConfigServer.xml 
    для работы с почтовым сервером и базой данных (файл CallSelectorConfig.xml необходимо настраивать 
    в директории инсталляции или в папке CallSelector/Config проекта CallSelector с последующей реинсталляцией )
5. Запустить сервисную службу CallSelector
6. Запустить проект CallSelectorWeb и зайти на страницу тестового сайта.
     ( К сожалению, предсказуемая работа возможна пока только в браузерах Опера 11.61 и Фаерфокс 10.0.2 )
7. Завершить выполнение приложения CallSelectorWeb и службы CallSelector





