# Obliksoft_Test_Task

<img width="640" alt="1cb6501a-32e2-48ed-957f-43c21ab2850b" src="https://user-images.githubusercontent.com/96773973/148698695-ad3d8752-d346-4af9-88a6-678ac2522ca3.png">

На основе данных в таблице нужно сделать расчеты в хранимой процедуре (скрипт таблицы MSSQL прилагается):
1. Надо разделить данные на отдельные прогулки (прогулка считается новой если промежуток времени между последним сигналом от 30 минут);
2. Просчитать расстояние пройденное за каждую прогулку;
3. Просчитать время каждой прогулку;
4. Просчитать сколько прошел за день и сколько времени всего за день гулял;
5. Создать вайбер бота, где по IMEI вывести Общую информацию по прогулке (количество, километраж, длительность) и ТОП 10 прогулок по пройденному расстоянию Вводим IMEI и получаем сообщение, как на фото. Нажимаем кнопку «Топ 10» и получаем инфо как на фото.

В данном случае - бот был создан и адаптирован под телеграмм.
