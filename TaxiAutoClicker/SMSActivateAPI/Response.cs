namespace TaxiAutoClicker.SMSActivateAPI
{
    public struct Response
    {
        public enum ResponseCodes
        {
            UNDEFINED = -1,
            ACCESS_BALANCE, // ACCESS_BALANCE:$balance (где $balance - баланс на счету
            NO_NUMBERS, // нет номеров
            NO_BALANCE, // закончился баланс
            ACCESS_NUMBER, // ACCESS_NUMBER:$id:$number - номер выдан ($id - id операции, $number - номер телефона)
            ACCESS_READY, // готовность номера подтверждена
            ACCESS_RETRY_GET, // ожидание нового смс
            ACCESS_ACTIVATION, // сервис успешно активирован
            ACCESS_CANCEL, // активация отменена
            STATUS_WAIT_CODE, // ожидание смс
            STATUS_WAIT_RETRY, // STATUS_WAIT_RETRY:$lastcode - ожидание уточнения кода (где - прошлый, неподошедший код)
            STATUS_WAIT_RESEND, // ожидание повторной отправки смс (софт должен нажать повторно выслать смс и выполнить изменение статуса на 6)
            STATUS_CANCEL, // активация отменена
            STATUS_OK, // STATUS_OK:$code - код получен (где - код активации)

            BAD_KEY, // Неверный API-ключ
            ERROR_SQL, // ошибка SQL-сервера
            BAD_ACTION, // некорректное действие
            BAD_SERVICE, // некорректное наименование сервиса
            BAD_STATUS, // некорректный статус
            BANNED, // BANNED:$time - аккаунт заблокирован до, где time - время формате YYYY-m-d H-i-s (2000-12-31 23-59-59)
            NO_ACTIVATION // id активации не существует
        }

        public ResponseCodes Code;
        public string Text; // Текст ответа сервиса.
        public string[] Params; // Параметры ответа.
        public bool IsError;
        
        public Response(string getRequest)
        {
            Text = "";
            IsError = true;
            Params = null;

            if (string.IsNullOrEmpty(getRequest))
            {
                Code = ResponseCodes.UNDEFINED;
                Text = "Ошибка получения ответа.";
                return;
            }

            string[] responseParams = getRequest.Split(':');

            if (responseParams.Length > 1)
            {
                Params = new string[responseParams.Length - 1];
                for (int i = 0; i < responseParams.Length - 1; i++)
                {
                    Params[i] = responseParams[i + 1];
                }
            }

            switch (responseParams[0])
            {
                case "ACCESS_BALANCE":
                {
                    Code = ResponseCodes.ACCESS_BALANCE;
                    IsError = false;
                    break;
                }
                case "NO_NUMBERS":
                {
                    Code = ResponseCodes.NO_NUMBERS;
                    IsError = true;
                    break;
                }
                case "NO_BALANCE":
                {
                    Code = ResponseCodes.NO_BALANCE;
                    IsError = true;
                    break;
                }
                case "ACCESS_NUMBER":
                {
                    Code = ResponseCodes.ACCESS_NUMBER;
                    IsError = false;
                    break;
                }
                case "ACCESS_READY":
                {
                    Code = ResponseCodes.ACCESS_READY;
                    IsError = false;
                    break;
                }
                case "ACCESS_RETRY_GET":
                {
                    Code = ResponseCodes.ACCESS_RETRY_GET;
                    IsError = false;
                    break;
                }
                case "ACCESS_ACTIVATION":
                {
                    Code = ResponseCodes.ACCESS_ACTIVATION;
                    IsError = false;
                    break;
                }
                case "ACCESS_CANCEL":
                {
                    Code = ResponseCodes.ACCESS_CANCEL;
                    IsError = false;
                    break;
                }
                case "STATUS_WAIT_CODE":
                {
                    Code = ResponseCodes.STATUS_WAIT_CODE;
                    IsError = false;
                    break;
                }
                case "STATUS_WAIT_RETRY":
                {
                    Code = ResponseCodes.STATUS_WAIT_RETRY;
                    IsError = false;
                    break;
                }
                case "STATUS_WAIT_RESEND":
                {
                    Code = ResponseCodes.STATUS_WAIT_RESEND;
                    IsError = false;
                    break;
                }
                case "STATUS_CANCEL":
                {
                    Code = ResponseCodes.STATUS_CANCEL;
                    IsError = false;
                    break;
                }
                case "STATUS_OK":
                {
                    Code = ResponseCodes.STATUS_OK;
                    IsError = false;
                    break;
                }

                case "BAD_KEY":
                {
                    Code = ResponseCodes.BAD_KEY;
                    IsError = true;
                    break;
                }
                case "ERROR_SQL":
                {
                    Code = ResponseCodes.ERROR_SQL;
                    IsError = true;
                    break;
                }
                case "BAD_ACTION":
                {
                    Code = ResponseCodes.BAD_ACTION;
                    IsError = true;
                    break;
                }
                case "BAD_SERVICE":
                {
                    Code = ResponseCodes.BAD_SERVICE;
                    IsError = true;
                    break;
                }
                case "BAD_STATUS":
                {
                    Code = ResponseCodes.BAD_STATUS;
                    IsError = true;
                    break;
                }
                case "BANNED":
                {
                    Code = ResponseCodes.BANNED;
                    IsError = true;
                    break;
                }
                case "NO_ACTIVATION":
                {
                    Code = ResponseCodes.NO_ACTIVATION;
                    IsError = true;
                    break;
                }
                default:
                {
                    Code = ResponseCodes.UNDEFINED;
                    IsError = true;
                    break;
                }
            }
        }
    }
}