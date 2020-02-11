using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace DatingApp.API.Helpers
{
    public class PhoneService
    {
        private const string accountSid = "!!!";
        private const string authToken = "!!!";
        public static void Send(string number, int code)
        {
            TwilioClient.Init(accountSid, authToken);
            MessageResource.Create(
                body: "DatingApp activation code: " + code.ToString(),
                from: new Twilio.Types.PhoneNumber("+12017334961"),
                to: new Twilio.Types.PhoneNumber(number)
            );
        }
    }
}