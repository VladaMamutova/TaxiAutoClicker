﻿using System.Threading;
using xNet;

namespace TaxiAutoClicker.SMSActivateAPI
{
    class Profile
    {
        private const string Url = "http://sms-activate.ru/stubs/handler_api.php";  // Ссылка для запросов
        private readonly string _api;  // Токен, использующийся при запросах
        public string Balance;
        public string Number;
        public string ActivationId;
        public string Code;

        public Profile(string accessToken)
        {
            _api = accessToken;
        }

        /// <summary>
        /// Запрос на получение баланса через сервис SMS-activate.
        /// </summary>
        /// <returns>Возвращает true, если запрос выполенен успешно, и false, если завершён с ошибкой.</returns>
        public bool GetBalance()
        {
            //http://sms-activate.ru/stubs/handler_api.php?api_key=$api_key&action=getBalance
            using (var request = new HttpRequest())
            {
                var urlParams = new RequestParams
                {
                    ["api_key"] = _api, ["action"] = "getBalance"
                };


                Response response =
                    new Response(request.Get(Url, urlParams).ToString());
                if (response.Code == Response.ResponseCodes.ACCESS_BALANCE)
                {
                    Balance = response.Params[0];
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Запрос на получение номера через сервис SMS-activate.
        /// </summary>
        /// <returns>Возвращает true, если запрос выполенен успешно, и false, если завершён с ошибкой.</returns>
        public Response GetNumber()
        {
            // http://sms-activate.ru/stubs/handler_api.php?api_key=$api_key&action=getNumber&service=$service&forward=$forward&operator=$operator&ref=$ref&country=$country
            using (var request = new HttpRequest())
            {
                var urlParams = new RequestParams
                {
                    ["api_key"] = _api,
                    ["action"] = "getNumber",
                    ["service"] = "ot",
                    ["forward"] = "0",
                    ["operator"] = "megafon",
                    ["country"] = "0"
                };

                // Любой другой сервис.
                // Не выполнять переадресацию.

                Response response;
                int maxTime = 10000;
                int pastTime = 0;
                do
                {
                    response =
                        new Response(request.Get(Url, urlParams).ToString());
                    Thread.Sleep(1000);
                    pastTime += 1000;
                } while (response.Code != Response.ResponseCodes.ACCESS_NUMBER && pastTime < maxTime);

                if (response.Code == Response.ResponseCodes.ACCESS_NUMBER)
                {
                    ActivationId = response.Params[0];
                    Number = response.Params[1].Substring(1); // Записываем номер без кода страны.
                }

                return response;
            }
        }

        /// <summary>
        /// Получение статуса активации.
        /// </summary>
        /// <returns>Полученный ответ от сервиса.</returns>
        public Response GetStatus()  // Получение статуса активации номера.
        {
            // http://sms-activate.ru/stubs/handler_api.php?api_key=$api_key&action=getStatus&id=$id
            using (var request = new HttpRequest())
            {
                var urlParams = new RequestParams
                {
                    ["api_key"] = _api,
                    ["action"] = "getStatus",
                    ["id"] = ActivationId
                };


                return new Response(request.Get(Url, urlParams).ToString());
            }
        }

        /// <summary>
        /// Запрос на изменение статуса активации.
        /// </summary>
        /// <returns>Возвращает true, если запрос выполенен успешно, и false, если завершён с ошибкой.</returns>
        private Response SetStatus(int status)
        {
            if (string.IsNullOrEmpty(ActivationId))
                return new Response();

            // http://sms-activate.ru/stubs/handler_api.php?api_key=$api_key&action=setStatus&status=$status&id=$id&forward=$forward
            using (var request = new HttpRequest())
            {
                var urlParams = new RequestParams
                {
                    ["api_key"] = _api,
                    ["action"] = "setStatus",
                    ["status"] = status.ToString(),
                    ["id"] = ActivationId
                };

                // Не выполнять переадресацию.
                //urlParams["forward"] = "0";

                return new Response(request.Get(Url, urlParams).ToString());
            }
        }

        // $status - статус активации:
        // -1 - отменить активацию
        // 1 - сообщить о готовности номера (смс на номер отправлено)
        // 3 - запросить еще один код (бесплатно)
        // 6 - завершить активацию(если был статус "код получен" - помечает успешно и завершает,
        // если был "подготовка" - удаляет и помечает ошибка, если был статус "ожидает повтора" - переводит активацию в ожидание смс)
        // 8 - сообщить о том, что номер использован и отменить активацию

        /// <summary>
        /// Запрос на изменение статуса активации.
        /// </summary>
        /// <returns>Возвращает true, если запрос выполенен успешно, и false, если завершён с ошибкой.</returns>
        public Response GetCode()
        {
            Response response = SetStatus(1);
            if (response.Code == Response.ResponseCodes.ACCESS_READY)
            {
                int maxTime = 1200000; // 20 минут = 1200 секунд = 1200 000 миллисекунд
                int pastTime = 0;
                do
                {
                    Thread.Sleep(1000);
                    pastTime += 1000;
                    response = GetStatus();
                } while (response.Code == Response.ResponseCodes.STATUS_WAIT_CODE && pastTime < maxTime);

                if (response.Code == Response.ResponseCodes.STATUS_OK)
                {
                    if (!int.TryParse(
                        response.Params[0].TrimStart(' ').Substring(0, 4),
                        out int code))
                    {
                        if (response.Params.Length > 1)
                        {
                            if (!int.TryParse(
                                response.Params[1].TrimStart(' ')
                                    .Substring(0, 4),
                                out code))
                                response.IsError = true;
                        }
                    }
                    Code = code.ToString().PadLeft(4, '0');
                }
                else
                {
                    response.IsError = true;
                }
            }
            else
            {
                response.IsError = true;
            }
            return response;
        }

        public void CompleteWorkWithNumber()
        {
            Response response = SetStatus(6);
            Code = response.Code.ToString();
        }
    }
}