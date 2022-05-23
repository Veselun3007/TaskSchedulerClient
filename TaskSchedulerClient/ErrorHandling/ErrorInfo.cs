using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace TaskShedulerDesktopClient.Data.Errors
{
    public class ErrorInfo 
    {
        private string serverError;
        public string ServerError 
        { 
            get 
            { 
                return serverError; 
            } 
            set 
            { 
                serverError = value; 
            } 
        }

        public static void SetValidationErrors(ErrorInfo errorInfo, 
            List<ValidationResult> validationResults)
        {
            Type myType = typeof(ErrorInfo);
            PropertyInfo[] props = myType.GetProperties();
            ClearErrors(errorInfo, props);
            foreach (ValidationResult error in validationResults)
            {
                foreach (PropertyInfo prop in props)
                {
                    string errorProp = Enumerable.ToArray(error.MemberNames)[0];
                    if (errorProp == prop.Name)
                    {
                        string mes = "";
                        try
                        {
                            mes = prop.GetValue(errorInfo).ToString();
                        }
                        catch { }
                        mes += error.ErrorMessage;
                        mes += " ";
                        prop.SetValue(errorInfo, mes);
                    }
                }
            }
        }

        private static void ClearErrors(ErrorInfo errorInfo, 
            PropertyInfo[] props)
        {
            foreach (PropertyInfo prop in props)
            {
                prop.SetValue(errorInfo, "");
            }
        }
        public void ClearErrors(ErrorInfo errorInfo)
        {
            Type myType = typeof(ErrorInfo);
            PropertyInfo[] props = myType.GetProperties();
            ClearErrors(errorInfo, props);
        }

        public static void SetServerErrors(ErrorInfo errorInfo, 
            HttpResponseMessage responseMessage)
        {
            Type myType = typeof(ErrorInfo);
            PropertyInfo[] props = myType.GetProperties();
            ClearErrors(errorInfo, props);

            int code = (int)responseMessage.StatusCode;

            if (code == 400)
            {
                string result = responseMessage.Content.ReadAsStringAsync().Result;
                Dictionary<string, string> dictionaryResult = 
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                try
                {
                    errorInfo.ServerError = dictionaryResult["error"];
                }
                catch (Exception)
                {
                    errorInfo.ServerError = "Сталася помилка";
                }
            }
            else
            {
                errorInfo.ServerError = "Помилка серверу. Повторіть, будь ласка, спробу";
            }
        }
    }
}
