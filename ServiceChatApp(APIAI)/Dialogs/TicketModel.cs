using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace ServiceChatApp_APIAI_.Dialogs
{
    [Serializable]

    public enum ContactOptions
    {
        Email,
        Phone
    }

    internal class TicketModel : IDialog<object>
    {
        string description;
        string contactOption;
        string retry_response = API_AI_Logger.API_Response("retry");

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(
                context,
                resume: description_option,
                prompt: "Please provide a detailed description of the problem are you facing",
                retry: retry_response );
        }

        private async Task description_option(IDialogContext context, IAwaitable<string> Description)
        {
            var res = await Description;
            description = res;
            var key_phrases_extracted = await KeyPhraseAnalytics.ExtractPhraseAsync(description);
            if(key_phrases_extracted.Contains("server") || key_phrases_extracted.Contains("database"))
            {
                //Console.WriteLine(key_phrases_extracted);
                PromptDialog.Text(
                context,
                resume: server_description,
                prompt: "Please provide server details",
                retry: retry_response);
            }
            else
            {
                PromptDialog.Choice(
                    context,
                    options: (IEnumerable<ContactOptions>)Enum.GetValues(typeof(ContactOptions)),
                    resume: contact_choice,
                    prompt: "How do we contact you?",
                    retry: retry_response
                    );
            }
            
        }

        private async Task contact_choice(IDialogContext context, IAwaitable<ContactOptions> contact_result)
        {
            var contactResponse= await contact_result;
           /**
            * Set the details in the method create incident calling method of SnowClass
            **/

        }

        private async Task server_description(IDialogContext context, IAwaitable<string> server_detail)
        {
            var res = await server_detail;
            if(res == "null")
            {
                await context.PostAsync("I have created a service ticket for the given details");

            }
            
        }
    }
}