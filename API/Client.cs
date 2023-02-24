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
        private static string BASE_URL = "https://stellabackend.pmclinic.pt/";

        public static Client Instance {
            get {
                if (_instance == null)
                    _instance = new Client();
                return _instance;
            }
        }
        private static Client _instance;

        private HttpClient _httpClient;

        private Client()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BASE_URL);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            LoadSettings();
        }

        /// <summary>
        /// Loads settings saved locally
        /// </summary>
        private void LoadSettings()
        {
            var settings = Properties.Settings.Default;
            if (settings.AuthToken != "")
            {
                _httpClient.DefaultRequestHeaders.Remove("x-stella-token");
                _httpClient.DefaultRequestHeaders.Add("x-stella-token", settings.AuthToken.Trim());
            }
        }

        /// <summary>
        /// Checks if there is already a defined token
        /// </summary>
        /// <param name="callback">Callback to be invoked with the result</param>
        public async void ValidateToken(Action<string> callback)
        {
            string result = null;

            var httpRes = await _httpClient.GetAsync("validate-token").ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(res);
                result = (string)obj["name"];
            }

            callback.Invoke(result);
        }

        /// <summary>
        /// Sets the token to be used on API calls
        /// </summary>
        /// <param name="token">Token string to set</param>
        public void SetToken(string token)
        {
            if (token != "")
            {
                _httpClient.DefaultRequestHeaders.Remove("x-stella-token");
                _httpClient.DefaultRequestHeaders.Add("x-stella-token", token.Trim());
            }

            Properties.Settings.Default.AuthToken = token;
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

            var httpRes = await _httpClient
                .PostAsync("add-service", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all services from the API
        /// </summary>
        public async void GetServices(Action<List<Service>> callback)
        {
            var httpRes = await _httpClient
                .GetAsync("get-services")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync().ConfigureAwait(false);
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
            var httpRes = await _httpClient
                .GetAsync($"delete-service?id={service.Id}")
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

            var httpRes = await _httpClient
                .PostAsync($"edit-service?id={id}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all customers from the API
        /// </summary>
        public async void GetCustomers(Action<List<Customer>> callback)
        {
            var httpRes = await _httpClient
                .GetAsync("get-customers")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync().ConfigureAwait(false);
                JArray arr = JArray.Parse(res);

                List<Customer> list = new List<Customer>();
                foreach (var obj in arr.Children<JObject>())
                {
                    Customer c = new Customer(
                        (string)obj["_id"],
                        (string)obj["card"],
                        (string)obj["name"],
                        (int?)obj["number"],
                        (string)obj["pin"],
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
            var httpRes = await _httpClient
                .GetAsync($"get-customer?id={customerId}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject obj = JObject.Parse(res);

                Customer c = new Customer(
                    (string)obj["_id"],
                    (string)obj["card"],
                    (string)obj["name"],
                    (int?)obj["number"],
                    (string)obj["pin"],
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
            var httpRes = await _httpClient
                .GetAsync($"get-customer-card?card={card}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject obj = JObject.Parse(res);

                Customer c = new Customer(
                    (string)obj["_id"],
                    (string)obj["card"],
                    (string)obj["name"],
                    (int?)obj["number"],
                    (string)obj["pin"],
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
        public async void AddCustomer(string name, string card, string pin, string address, string phone, string email, string gender, string birthdate, string notes, Action<bool> callback)
        {
            var body = new JObject();
            body["name"] = name;
            body["card"] = card;
            body["pin"] = pin;
            body["address"] = address;
            body["phone"] = phone;
            body["email"] = email;
            body["gender"] = gender;
            if(birthdate != "") body["birthdate"] = birthdate;
            body["notes"] = notes;

            var httpRes = await _httpClient
                .PostAsync("add-customer", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Removes a customer using the API
        /// </summary>
        public async void RemoveCustomer(Customer customer, Action<bool> callback)
        {
            var httpRes = await _httpClient
                .GetAsync($"delete-customer?id={customer.Id}")
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Edits customer using the API
        /// </summary>
        public async void EditCustomer(string id, string card, string pin, string name, string address, string phone, string email, string gender, string birthdate, string notes, Action<bool> callback)
        {
            var body = new JObject();
            body["name"] = name;
            body["card"] = card;
            body["pin"] = pin;
            body["address"] = address;
            body["phone"] = phone;
            body["email"] = email;
            body["gender"] = gender;
            if (birthdate != "") body["birthdate"] = birthdate;
            body["notes"] = notes;

            var httpRes = await _httpClient
                .PostAsync($"edit-customer?id={id}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
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
            body["serviceName"] = serviceName;
            body["quantity"] = quantity;

            var httpRes = await _httpClient
                .PostAsync("add-records", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all records for the given customer from the API
        /// </summary>
        public async void GetRecords(string customerId, Action<List<AccumulatedRecord>> callback)
        {
            var httpRes = await _httpClient
                .GetAsync($"get-records?customer={customerId}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync().ConfigureAwait(false);
                JArray arr = JArray.Parse(res);

                var dict = new Dictionary<string, AccumulatedRecord>();
                foreach (var obj in arr.Children<JObject>())
                {
                    string serviceName = (string)obj["serviceName"];
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
            var httpRes = await _httpClient
                .GetAsync($"delete-record?id={recordId}")
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
            body["serviceName"] = serviceName;
            body["date"] = $"{date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}";
            if(single) body["single"] = true;
            body["notes"] = notes;

            var httpRes = await _httpClient
                .PostAsync("add-transaction", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the list of all transactions for the given customer from the API
        /// </summary>
        public async void GetTransactions(string customerId, Action<List<Transaction>> callback)
        {
            var httpRes = await _httpClient
                .GetAsync($"get-transactions?customer={customerId}")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync().ConfigureAwait(false);
                JArray arr = JArray.Parse(res);

                List<Transaction> list = new List<Transaction>();
                foreach (var obj in arr.Children<JObject>())
                {
                    Transaction t = new Transaction(
                        (string)obj["_id"],
                        (DateTime?)obj["date"],
                        (string)obj["serviceName"],
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
            var httpRes = await _httpClient
                .GetAsync($"delete-transaction?id={t.Id}")
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

            var httpRes = await _httpClient
                .PostAsync($"edit-transaction-notes?id={transactionId}", new StringContent(body.ToString(), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            callback(httpRes.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Retrieves the statistics from the API
        /// </summary>
        public async void GetStats(Action<Stats> callback)
        {
            var httpRes = await _httpClient
                .GetAsync("get-stats")
                .ConfigureAwait(false);

            if (httpRes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string res = await httpRes.Content.ReadAsStringAsync().ConfigureAwait(false);
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
