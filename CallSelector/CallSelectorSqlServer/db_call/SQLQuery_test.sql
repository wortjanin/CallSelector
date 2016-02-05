USE db_call
GO


                  SELECT TOP (150)
                     O.mail AS operator_mail,
                     PC.phone,
                     PC.date_start,
                     PC.date_interval,
                     PC.sender_mail, 
                     PC.file_name
                    FROM phone_call PC
                    JOIN operator O ON(PC.id_operator = O.id_operator)
                     WHERE 1=1 AND '2012-03-13 0:00:00' <= PC.date_start AND PC.date_start <= '14.03.2012 0:00:00'
                    ORDER BY (PC.date_start) DESC


SELECT * FROM operator
GO

SELECT * FROM phone_call
WHERE date_start >= '14/03/2012 00:00:00' 
GO

INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 19:57:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')
INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 20:07:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')
INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 20:17:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')
INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 20:27:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')
INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 20:37:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')
INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 20:47:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')
INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 20:57:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')
INSERT INTO phone_call(id_operator, phone, date_start, date_interval, sender_mail, file_name)
VALUES 
(1,	79199607236,	'13/03/2012 21:07:48.000',	'00:01:30',	'achernoivanov@gmail.com',	'201203132048489475_record_6Feb2012_11h15m18s.mp3')



DELETE FROM phone_call
GO

SELECT * FROM failed_message
GO

-- Do not forget to create login: call_selector / call_selector_pass
-- and give it appropriate privelegies to read and write tables: 
--    db_call.operator, db_call.phone_call, db_call.failed_message