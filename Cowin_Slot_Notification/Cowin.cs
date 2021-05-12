using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Cowin_Slot_Notification
{
    class Cowin
    {
        public async static Task<VaccineDetails> GetVaccineDetailsAsync(string pincode, string date)
        {
            try
            {

                var http = new HttpClient();
                var url = String.Format("https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin?pincode={0}&date={1}", pincode, date);
                var response = await http.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(VaccineDetails));

                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
                var data = (VaccineDetails)serializer.ReadObject(ms);

                return data;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return new VaccineDetails();
            }
        }

        public async Task<VaccineDetails> GetDataAsync(string pincode, string date)
        {
            
            // Create a RootObject which will contain the results of the GetWeather API call
            VaccineDetails details = await Cowin.GetVaccineDetailsAsync(pincode, date);

            return details;
        }
    }
}
