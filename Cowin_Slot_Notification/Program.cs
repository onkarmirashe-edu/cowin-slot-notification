using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Runtime.Serialization.Json;
using System.IO;
using SendMailNamespace;

namespace Cowin_Slot_Notification
{

    class Program
    {

        /// <summary>
        /// smtp address property for SMTP
        /// </summary>
        static string smtpAddress = "smtp.gmail.com";

        /// <summary>
        /// port number property for SMTP
        /// </summary>
        static int portNumber = 587;

        /// <summary>
        /// enable ssl property for SMTP
        /// </summary>
        static bool enableSSL = true;

        /// <summary>
        /// subject for mail to be sent
        /// </summary>
        static string subject = "Vaccine Slot Availability Notification";

        /// <summary>
        /// body format for the mail to be sent
        /// </summary>
        static string body = "Hello All, <br/>Please find below vaccine slot availabilit details.<br/>Please Use Below link for booking appointment :<br/><a href='https://selfregistration.cowin.gov.in/'>https://selfregistration.cowin.gov.in/</a><br/>";

        /// <summary>
        /// Configuration data should be loaded once at the start of the application
        /// if the configuration is changed application should be restarted
        /// </summary>
        static ApplicationConfiguration applicationConfiguration;

        /// <summary>
        /// Main method star point for the application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var currDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileName = "configuration.txt";
            var configFile = Path.Combine(currDir, fileName);
            var serializer = new DataContractJsonSerializer(typeof(ApplicationConfiguration));
            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(configFile, FileMode.Open, FileAccess.Read))
                file.CopyTo(ms);
            ms.Position = 0;
            applicationConfiguration = serializer.ReadObject(ms) as ApplicationConfiguration;

            var apiCallsPerExecution = applicationConfiguration.Configurations.Count;

            // 100 calls per 5 minutes per IP. Means
            var apiCallTimeDiff = 3 * 1000;
            Timer timer = new Timer();
            timer.Interval = apiCallsPerExecution * apiCallTimeDiff;
            Program obj = new Program();
            timer.Elapsed += new ElapsedEventHandler(obj.timer_Elapsed);
            timer.Start();
            Console.Read();
        }

        /// <summary>
        /// timer elapsed function that is executed after every interval aopunt of time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var configuration in applicationConfiguration.Configurations)
                {
                    Cowin cowin = new Cowin();
                    VaccineDetails details = cowin.GetDataAsync(configuration.Pincode, DateTime.Now.ToString("dd-MM-yyyy")).Result;
                    ProcessVaccineDetails(details, configuration);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }

        /// <summary>
        /// Process vaccine slot data received from Web API
        /// </summary>
        /// <param name="details"></param>
        /// <param name="configuration"></param>
        private static void ProcessVaccineDetails(VaccineDetails details, Configuration configuration)
        {
            var centersHavingSlots = GetCenterDetails(details, configuration);
            if (centersHavingSlots.Any())
            {

                var mailBody = GetMailBody(centersHavingSlots, configuration);

                try
                {
                    SendMail sendMail = new SendMail();
                    SmtpInfo info = new SmtpInfo() { Body = mailBody, PortNumber = portNumber, EnableSSL = enableSSL, EmailToAddress = configuration.To_email, SmtpAddress = smtpAddress, Subject = subject };
                    sendMail.SendEmail(info);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                }
            }
        }

        /// <summary>
        /// return centers as per the filer criteria
        /// </summary>
        /// <param name="details"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static List<Center> GetCenterDetails(VaccineDetails details, Configuration configuration)
        {
            return details.centers.Where(x => (configuration.Name.Any() == false || configuration.Name.Contains(x.name, StringComparer.InvariantCultureIgnoreCase)) && x.sessions.Any(y => y.available_capacity > 0 && (configuration.Min_age_limit == 0 || configuration.Min_age_limit == y.min_age_limit) && (configuration.Dose.Trim() == "-" || string.IsNullOrEmpty(configuration.Dose) || (configuration.Dose.Trim() == "1" && y.available_capacity_dose1 > 0) || (configuration.Dose.Trim() == "2" && y.available_capacity_dose2 > 0)))).ToList();
        }

        /// <summary>
        /// get formatted mail body from the data and configuration elements
        /// </summary>
        /// <param name="centersHavingSlots"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static string GetMailBody(IEnumerable<Center> centersHavingSlots, Configuration configuration)
        {
            StringBuilder data = new StringBuilder(body);
            foreach (var item in centersHavingSlots)
            {
                data.AppendLine("<br/>****************************************************");
                data.AppendLine("<br/>****************************************************");
                data.AppendLine("<br/>");
                data.AppendLine("<br/>Center Details :");
                data.AppendLine("<br/>Name : " + item.name);
                data.AppendLine("<br/>Address : " + item.address);
                data.AppendLine("<br/>Pincode : " + item.pincode);
                data.AppendLine("<br/>Fee : " + item.fee_type);

                foreach (var session in item.sessions.Where(x => x.available_capacity > 0 && (configuration.Min_age_limit == 0 || configuration.Min_age_limit == x.min_age_limit) && (configuration.Dose.Trim() == "-" || string.IsNullOrEmpty(configuration.Dose) || (configuration.Dose.Trim() == "1" && x.available_capacity_dose1 > 0) || (configuration.Dose.Trim() == "2" && x.available_capacity_dose2 > 0))))
                {
                    data.AppendLine("<br/>");
                    data.AppendLine("<br/>------------------------------------------------");
                    data.AppendLine("<br/>");
                    data.AppendLine("<br/>Session Details :");
                    data.AppendLine("<br/>Date : " + session.date);
                    data.AppendLine("<br/>Available Capacity : " + session.available_capacity);
                    data.AppendLine("<br/>Minimum Age Limit : " + session.min_age_limit);
                    data.AppendLine("<br/>Available Capacity Dose 1 : " + session.available_capacity_dose1);
                    data.AppendLine("<br/>Available Capacity Dose 2 : " + session.available_capacity_dose2);
                    data.AppendLine("<br/>Vaccine : " + session.vaccine);
                    data.AppendLine("<br/>");
                    data.AppendLine("<br/>------------------------------------------------");
                    data.AppendLine("<br/>");
                }
            }

            data.AppendLine("<br/>");
            data.AppendLine("<br/>****************************************************");
            data.AppendLine("<br/>****************************************************");
            data.AppendLine("<br/>");

            return data.ToString();
        }
    }
}
