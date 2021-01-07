using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Threading;

namespace CurrencyConverter
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MainViewModel : BaseViewModel
    {
        HttpClient http = new HttpClient();
        private string _message;
        private DateTime _date; 
        private Currency _selectedCurrency1;
        private Currency _selectedCurrency2;
        private string _currencyResult1;
        private string _currencyResult2;
        private bool Flag = true;
        public Root result { get; set; }
        public string Message 
        { 
            get { return _message; } 
            set { _message = value; OnPropertyChanged(); } 
        }
        public Command SetCurrency { get; set; }
        public DateTime Date 
        {
            get { return _date; } 
            set { _date = value; OnPropertyChanged(); setCurrency(); } 
        }
        private ObservableCollection<Currency> _currencyNames;
        public ObservableCollection<Currency> CurrencyNames 
        {
            get { return _currencyNames; } 
            set { _currencyNames = value; OnPropertyChanged(); } 
        }
        public Currency SelectedCurrency1
        {
            get { return _selectedCurrency1; }
            set
            {
                if (value == null)
                    CurrencyResult1 = "";
                else
                {
                    _selectedCurrency1 = value;
                    setCurrency1();
                }
                
                OnPropertyChanged();
            }
        }
        
        public Currency SelectedCurrency2
        {
            get { return _selectedCurrency2; }
            set
            {
                if (value == null)
                    CurrencyResult2 = "";
                else
                {
                    _selectedCurrency2 = value;
                    setCurrency2();
                }
                
                OnPropertyChanged();
            }
        }
        public string CurrencyResult1
        {
            get { return _currencyResult1; }
            set
            {
                _currencyResult1 = value;
                if (!string.IsNullOrEmpty(value) && SelectedCurrency1 != null && SelectedCurrency2 != null && Flag)
                {
                    Flag = false;
                    setCurrency2();
                }
                else
                    Flag = true;

                OnPropertyChanged(nameof(CurrencyResult1));
            }
        }
        public string CurrencyResult2
        {
            get { return _currencyResult2; }
            set
            {
                _currencyResult2 = value;
                if (!string.IsNullOrEmpty(value) && SelectedCurrency1 != null && SelectedCurrency2 != null && Flag)
                {
                    Flag = false;
                    setCurrency1();
                }
                else
                    Flag = true;

                OnPropertyChanged(nameof(CurrencyResult2));
            }
        }
        public void setCurrency2()
        {
            if (!string.IsNullOrEmpty(CurrencyResult1))
            {
                CurrencyResult2 = Math.Round(((SelectedCurrency1.Value * Convert.ToDouble(CurrencyResult1)) / SelectedCurrency2.Value), 4).ToString();
            }
        }
        public void setCurrency1()
        {
            if (!string.IsNullOrEmpty(CurrencyResult2))
            {
                CurrencyResult1 = Math.Round(((SelectedCurrency2.Value * Convert.ToDouble(CurrencyResult2)) / SelectedCurrency1.Value), 4).ToString();
            }
        }
        public MainViewModel()
        {
            CurrencyNames = new ObservableCollection<Currency>();
            CurrencyResult1 = "";
            CurrencyResult2 = "";
            Date = DateTime.Now;
        }

        private async void setCurrency()
        {
            string uri = "";
            var date = DateTime.Now;

            if (Date.ToString("dd MM yyyy") == date.ToString("dd MM yyyy"))
            {
                uri = $"https://www.cbr-xml-daily.ru/archive/{date.Year}/{date.Month}/{date.Day}/daily_json.js";
            }
            else
            {
                uri = $"https://www.cbr-xml-daily.ru/archive/{Date.Year}/{Date.Month}/{Date.Day}/daily_json.js";
            }
            var res = await http.GetAsync(uri);

            if (res.IsSuccessStatusCode)
            {
                string content = await res.Content.ReadAsStringAsync();

                JObject jObject = JObject.Parse(content);
                result = jObject.ToObject<Root>();
                setCurNameSelector();

                Message = "Курс на " + Date.ToString("dd MMMM yyyy");
            }
            else
            {
                date = Date;
                while (!res.IsSuccessStatusCode)
                {
                    date = date.AddDays(-1);
                    uri = $"https://www.cbr-xml-daily.ru/archive/{date.Year}/{date.Month}/{date.Day}/daily_json.js";

                    res = await http.GetAsync(uri);
                }
                Date = date;
            }
        }
        

        public void setCurNameSelector()
        {
            Currency selectCurrency1 = null;
            Currency selectCurrency2 = null;
            if (SelectedCurrency1 != null && SelectedCurrency2 != null)
            {
                selectCurrency1 = SelectedCurrency1;
                selectCurrency2 = SelectedCurrency2;
            }
            CurrencyNames.Clear();
            result.Valute.RUB = new Currency();
            result.Valute.RUB.CharCode = "RUB";
            result.Valute.RUB.Value = 1;
            CurrencyNames.Add(result.Valute.RUB);
            CurrencyNames.Add(result.Valute.USD);
            CurrencyNames.Add(result.Valute.EUR);
            CurrencyNames.Add(result.Valute.AUD);
            CurrencyNames.Add(result.Valute.AZN);
            CurrencyNames.Add(result.Valute.GBP);
            CurrencyNames.Add(result.Valute.AMD);
            CurrencyNames.Add(result.Valute.BYN);
            CurrencyNames.Add(result.Valute.BGN);
            CurrencyNames.Add(result.Valute.BRL);
            CurrencyNames.Add(result.Valute.AMD);
            CurrencyNames.Add(result.Valute.HUF);
            CurrencyNames.Add(result.Valute.HKD);
            CurrencyNames.Add(result.Valute.DKK);
            CurrencyNames.Add(result.Valute.INR);
            CurrencyNames.Add(result.Valute.KZT);
            CurrencyNames.Add(result.Valute.CAD);
            CurrencyNames.Add(result.Valute.KGS);
            CurrencyNames.Add(result.Valute.CNY);
            CurrencyNames.Add(result.Valute.MDL);
            CurrencyNames.Add(result.Valute.NOK);
            CurrencyNames.Add(result.Valute.PLN);
            CurrencyNames.Add(result.Valute.RON);
            CurrencyNames.Add(result.Valute.XDR);
            CurrencyNames.Add(result.Valute.SGD);
            CurrencyNames.Add(result.Valute.TJS);
            CurrencyNames.Add(result.Valute.TRY);
            CurrencyNames.Add(result.Valute.TMT);
            CurrencyNames.Add(result.Valute.UZS);
            CurrencyNames.Add(result.Valute.UAH);
            CurrencyNames.Add(result.Valute.CZK);
            CurrencyNames.Add(result.Valute.SEK);
            CurrencyNames.Add(result.Valute.CHF);
            CurrencyNames.Add(result.Valute.ZAR);
            CurrencyNames.Add(result.Valute.KRW);
            CurrencyNames.Add(result.Valute.JPY);
            if (selectCurrency1 != null && selectCurrency2 != null)
            {
                foreach (Currency CurrencyName in CurrencyNames)
                {
                    if (CurrencyName.CharCode == selectCurrency1.CharCode)
                    {
                        SelectedCurrency1 = CurrencyName;
                    }
                    if (CurrencyName.CharCode == selectCurrency2.CharCode)
                    {
                        SelectedCurrency2 = CurrencyName;
                    }
                }
            }
        }
        public class Root
        {
            public DateTime Date { get; set; }
            public DateTime PreviousDate { get; set; }
            public string PreviousURL { get; set; }
            public DateTime Timestamp { get; set; }
            public Valute Valute { get; set; }
        }
        public class Valute
        {
            public Currency RUB { get; set; }
            public Currency USD { get; set; }
            public Currency EUR { get; set; }
            public Currency AUD { get; set; }
            public Currency AZN { get; set; }
            public Currency GBP { get; set; }
            public Currency AMD { get; set; }
            public Currency BYN { get; set; }
            public Currency BGN { get; set; }
            public Currency BRL { get; set; }
            public Currency HUF { get; set; }
            public Currency HKD { get; set; }
            public Currency DKK { get; set; }
            public Currency INR { get; set; }
            public Currency KZT { get; set; }
            public Currency CAD { get; set; }
            public Currency KGS { get; set; }
            public Currency CNY { get; set; }
            public Currency MDL { get; set; }
            public Currency NOK { get; set; }
            public Currency PLN { get; set; }
            public Currency RON { get; set; }
            public Currency XDR { get; set; }
            public Currency SGD { get; set; }
            public Currency TJS { get; set; }
            public Currency TRY { get; set; }
            public Currency TMT { get; set; }
            public Currency UZS { get; set; }
            public Currency UAH { get; set; }
            public Currency CZK { get; set; }
            public Currency SEK { get; set; }
            public Currency CHF { get; set; }
            public Currency ZAR { get; set; }
            public Currency KRW { get; set; }
            public Currency JPY { get; set; }
        }
        public class Currency
        {
            public string ID { get; set; }
            public string NumCode { get; set; }
            public string CharCode { get; set; }
            public int Nominal { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
            public double Previous { get; set; }
        }
    }
}
