﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ServiceChatApp_APIAI_.Dialogs.ScorableDialog
{
    public class ChatResetScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask task;

        public ChatResetScorable(IDialogTask task)
        {
            SetField.NotNull(out this.task, nameof(task), task);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            //this.task.Reset();
            var message = item as IMessageActivity;

            if (message != null)
            {

                var root = new RootDialog();

                //var ticketForm = new RaiseDialog();

                var interruption = root.Void<object, IMessageActivity>();

                task.Call(interruption, null);

                await task.PollAsync(token);

                this.task.Reset();
            }

        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.Equals("cancel", StringComparison.InvariantCultureIgnoreCase) || message.Text.Equals("reset", StringComparison.InvariantCultureIgnoreCase) ||
                    message.Text.Equals("restart", StringComparison.InvariantCultureIgnoreCase) || message.Text.Equals("start again", StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }
            return null;
        }
    }
}