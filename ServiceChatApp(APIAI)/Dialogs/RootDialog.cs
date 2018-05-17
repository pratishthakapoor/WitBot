using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiAiSDK;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;

namespace ServiceChatApp_APIAI_.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string retry_response = API_AI_Logger.API_Response("retry");

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            // Return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            string response = API_AI_Logger.API_Response(activity.Text);
            string action_response = API_AI_Logger.API_Connection_Action(activity.Text);

            await context.PostAsync(response);
            if(action_response.Contains("Raise Ticket-next"))
            {
                PromptDialog.Confirm(
                    context,
                    resume: ResponseOption,
                    prompt: "Do you wish to check that out",
                    retry: retry_response
                    );
        
            }

            if(action_response.Contains("Raise Ticket-repeat"))
            {

            }

            else
            {
                PromptDialog.Text(
                context,
                resume: MenuOption,
                prompt: "I can \n \n 1. Raise an incident ticket. \n \n 2. Check the status of previous raise ticket.",
                retry: "Please try agin, as some problem occured");
            }
            

            //context.Wait(MessageReceivedAsync);
        }

        private async Task ResponseOption(IDialogContext context, IAwaitable<bool> result)
        {
            var confirmation = await result;
            PromptDialog.Text(
                context,
                resume: DisplayTicketStatus,
                prompt:"Please provide the ticket number for which you want to check the status",
                retry: retry_response);
        }

        private async Task DisplayTicketStatus(IDialogContext context, IAwaitable<string> result)
        {
            var incidentTokenNumber = await result;

            string statusDetail = Logger.RetrieveIncidentServiceNow(incidentTokenNumber);
        }

        private async Task MenuOption(IDialogContext context, IAwaitable<string> result)
        {
            var res = await result;

            string menu_response = API_AI_Logger.API_Response(res);

            await context.PostAsync(menu_response);
            context.Call(child: new TicketModel(), resume: childDialogcomplete);
        }

        private async Task childDialogcomplete(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(this);
        }
    }
}