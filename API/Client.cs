using Newtonsoft.Json.Linq;
using Stella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Stella.API
{
    public class Client
    {
        private static string BASE_URL = "https://stella-backend-free.herokuapp.com/";

        public static Client Instance {
            get {
                if (_instance == null)
                    _instance = new Client();
                return _instance;
            }
        }
        private static Client _instance;

        private HttpClient httpClient;
        private string token = null;

        private Client()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BASE_URL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            LoadSettings();
        }

        /// <summary>
        /// Loads settings saved locally
        /// </summary>
        private void LoadSettings()
        {
            var settings = Properties.Settings.Default;
            token = settings.AuthToken == "" ? null : settings.AuthToken.Trim();
        }

        /// <summary>
        /// Checks if there is already a defined token
        /// </summary>
        /// <param name="callback">Callback to be invoked with the result</param>
        public async void ValidateToken(Action<string> callback)
        {
            string result = null;

            if (token != null)
            {
                var httpRes = await httpClient.GetAsync($"validate-token?token={token}").ConfigureAwait(false);

                if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string res = await httpRes.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(res);
                    result = (string)obj["name"];
                }
            }            

            callback.Invoke(result);
        }

        /// <summary>
        /// Sets the token to be used on API calls
        /// </summary>
        /// <param name="token">Token string to set</param>
        public void SetToken(string token)
        {
            this.token = token == "" ? null : token.Trim();
            Properties.Settings.Default.AuthToken = token.Trim();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Adds service using the API.
        /// </summary>
        /// <param name="name">Name of the Service</param>
        /// <param name="notes">Notes of the Service</param>
        /// <returns>True if added successfully, false otherwise</returns>
        public async void AddService(string name, string notes, Action<bool> callback)
        {
            var body = new JObject();
            body["name"] = name;
            body["notes"] = notes;

            var httpRes = await httpClient
                .PostAsync($"add-service?token={token}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all services from the API
        /// </summary>
        public async void GetServices(Action<List<Service>> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"get-services?token={token}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JArray arr = JArray.Parse(res);

                List<Service> list = new List<Service>();
                foreach (var obj in arr.Children<JObject>())
                {
                    Service s = new Service((string)obj["_id"], (string)obj["name"], (string)obj["notes"]);
                    list.Add(s);
                }

                callback(list);
            }
            else
            {
                callback(null);
            }
        }

        /// <summary>
        /// Removes a service using the API
        /// </summary>
        public async void RemoveService(Service service, Action<bool> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"delete-service?token={token}&id={service.Id}")
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Edits service using the API
        /// </summary>
        public async void EditService(string id, string name, string notes, Action<bool> callback)
        {
            var body = new JObject();
            body["name"] = name;
            body["notes"] = notes;

            var httpRes = await httpClient
                .PostAsync($"edit-service?token={token}&id={id}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all customers from the API
        /// </summary>
        public async void GetCustomers(Action<List<Customer>> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"get-customers?token={token}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JArray arr = JArray.Parse(res);

                List<Customer> list = new List<Customer>();
                foreach (var obj in arr.Children<JObject>())
                {
                    Customer c = new Customer(
                        (string)obj["_id"],
                        (string)obj["card"],
                        (string)obj["name"],
                        (int?)obj["number"],
                        (string)obj["address"],
                        (string)obj["phone"],
                        (string)obj["email"],
                        (string)obj["gender"],
                        (DateTime?)obj["birthdate"],
                        (string)obj["notes"]);

                    list.Add(c);
                }

                callback(list);
            }
            else
            {
                callback(null);
            }
        }

        /// <summary>
        /// Retrieves a single customer from the API
        /// </summary>
        public async void GetCustomer(string customerId, Action<Customer> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"get-customer?token={token}&id={customerId}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(res);

                Customer c = new Customer(
                    (string)obj["_id"],
                    (string)obj["card"],
                    (string)obj["name"],
                    (int?)obj["number"],
                    (string)obj["address"],
                    (string)obj["phone"],
                    (string)obj["email"],
                    (string)obj["gender"],
                    (DateTime?)obj["birthdate"],
                    (string)obj["notes"]
                );

                callback(c);
            }
            else
            {
                callback(null);
            }
        }

        /// <summary>
        /// Retrieves a single customer using card from the API
        /// </summary>
        public async void GetCustomerCard(string card, Action<Customer> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"get-customer-card?token={token}&card={card}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(res);

                Customer c = new Customer(
                    (string)obj["_id"],
                    (string)obj["card"],
                    (string)obj["name"],
                    (int?)obj["number"],
                    (string)obj["address"],
                    (string)obj["phone"],
                    (string)obj["email"],
                    (string)obj["gender"],
                    (DateTime?)obj["birthdate"],
                    (string)obj["notes"]
                );

                callback(c);
            }
            else
            {
                callback(null);
            }
        }

        /// <summary>
        /// Adds customer using the API.
        /// </summary>
        /// <returns>True if added successfully, false otherwise</returns>
        public async void AddCustomer(string name, string card, int? number, string address, string phone, string email, string gender, string birthdate, string notes, Action<bool> callback)
        {
            var body = new JObject();
            body["name"] = name;
            body["card"] = card;
            if(number != null) body["number"] = number;
            body["address"] = address;
            body["phone"] = phone;
            body["email"] = email;
            body["gender"] = gender;
            if(birthdate != "") body["birthdate"] = birthdate;
            body["notes"] = notes;

            var httpRes = await httpClient
                .PostAsync($"add-customer?token={token}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Removes a customer using the API
        /// </summary>
        public async void RemoveCustomer(Customer customer, Action<bool> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"delete-customer?token={token}&id={customer.Id}")
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Edits customer using the API
        /// </summary>
        public async void EditCustomer(string id, string card, string name, int? number, string address, string phone, string email, string gender, string birthdate, string notes, Action<bool> callback)
        {
            var body = new JObject();
            body["name"] = name;
            body["card"] = card;
            if (number != null) body["number"] = number;
            body["address"] = address;
            body["phone"] = phone;
            body["email"] = email;
            body["gender"] = gender;
            if (birthdate != "") body["birthdate"] = birthdate;
            body["notes"] = notes;

            var httpRes = await httpClient
                .PostAsync($"edit-customer?token={token}&id={id}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds records using the API.
        /// </summary>
        /// <returns>True if added successfully, false otherwise</returns>
        public async void AddRecords(string customerId, string serviceName, int quantity, Action<bool> callback)
        {
            var body = new JObject();
            body["customer"] = customerId;
            body["service"] = serviceName;
            body["quantity"] = quantity;

            var httpRes = await httpClient
                .PostAsync($"add-records?token={token}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all records for the given customer from the API
        /// </summary>
        public async void GetRecords(string customerId, Action<List<AccumulatedRecord>> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"get-records?token={token}&customer={customerId}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JArray arr = JArray.Parse(res);

                var dict = new Dictionary<string, AccumulatedRecord>();
                foreach (var obj in arr.Children<JObject>())
                {
                    string serviceName = (string)obj["service"];
                    string cid = (string)obj["customer"];
                    string rid = (string)obj["_id"];

                    if (!dict.ContainsKey(serviceName))
                        dict[serviceName] = new AccumulatedRecord(cid, serviceName);

                    dict[serviceName].Ids.Add(rid);
                }

                callback(dict.Values.ToList());
            }
            else
            {
                callback(null);
            }
        }

        /// <summary>
        /// Removes a record using the API
        /// </summary>
        public async void RemoveRecord(string recordId, Action<bool> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"delete-record?token={token}&id={recordId}")
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds transaction using the API.
        /// </summary>
        /// <returns>True if added successfully, false otherwise</returns>
        public async void AddTransaction(string customerId, string serviceName, DateTime date, bool single, string notes, Action<bool> callback)
        {
            var body = new JObject();
            body["customer"] = customerId;
            body["service"] = serviceName;
            body["date"] = $"{date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}";
            if(single) body["single"] = true;
            body["notes"] = notes;

            var httpRes = await httpClient
                .PostAsync($"add-transaction?token={token}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all transactions for the given customer from the API
        /// </summary>
        public async void GetTransactions(string customerId, Action<List<Transaction>> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"get-transactions?token={token}&customer={customerId}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JArray arr = JArray.Parse(res);

                List<Transaction> list = new List<Transaction>();
                foreach (var obj in arr.Children<JObject>())
                {
                    Transaction t = new Transaction(
                        (string)obj["_id"],
                        (DateTime?)obj["date"],
                        (string)obj["service"],
                        (bool)obj["single"],
                        (string)obj["notes"]);

                    list.Add(t);
                }

                callback(list);
            }
            else
            {
                callback(null);
            }
        }

        /// <summary>
        /// Removes a transaction using the API
        /// </summary>
        public async void RemoveTransaction(Transaction t, Action<bool> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"delete-transaction?token={token}&id={t.Id}")
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds transaction using the API.
        /// </summary>
        /// <returns>True if added successfully, false otherwise</returns>
        public async void EditTransactionNotes(string notes, string transactionId, Action<bool> callback)
        {
            var body = new JObject();
            body["notes"] = notes;

            var httpRes = await httpClient
                .PostAsync($"edit-transaction-notes?token={token}&id={transactionId}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the statistics from the API
        /// </summary>
        public async void GetStats(Action<Stats> callback)
        {
            var httpRes = await httpClient
                .GetAsync($"get-stats?token={token}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(res);

                Stats s = new Stats(
                    (string)obj["services"],
                    (string)obj["customers"],
                    (string)obj["transactions"]);

                callback(s);
            }
            else
            {
                callback(null);
            }
        }
    }
}
