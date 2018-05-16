using ApiAiSDK;
using System;

namespace ServiceChatApp_APIAI_.Dialogs
{
    internal class API_AI_Logger
    {
        

        internal static string API_Response(string text)
        {
            ApiAi apiAi;

            var config = new AIConfiguration(Constants.APIAI_SUBSCRIPTION_ID, SupportedLanguage.English);

            apiAi = new ApiAi(config);

            var resposne = apiAi.TextRequest(text);
            return resposne.Result.Fulfillment.Speech;
        }
    }
}