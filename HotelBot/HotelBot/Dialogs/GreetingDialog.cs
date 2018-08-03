using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HotelBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        //override method to async receive messages
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I am Wil's bot.");
            await Respond(context);

            //make sure that the name of the user is saved against the state on the UserData poproperty bag
            context.Wait(MessageReceivedAsync);

        }

        private static async Task Respond(IDialogContext context)
        {
            var userName = String.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);
            if (string.IsNullOrEmpty(userName))
            {
                //query the user to pass their name
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                await context.PostAsync(String.Format("Hi {0}. How can I help you today?", userName));
            }
        }

        //Async method to be run after sending a message to the client
        //the IMessageActivity contains the text attribute which is what the user has typed
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var userName = String.Empty;
            var getName = false;
            context.UserData.TryGetValue<string>("Name", out userName);
            context.UserData.TryGetValue<bool>("GetName", out getName);

            //check that the user name is already on the properties bag on the 'state'
          if (getName)
            {
                //get the username if not already saved on the state and save it
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
                await Respond(context);
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                //end the dialogue if the bot has the user anme of the person making the requests
                context.Done(message);
            }           
        }
    }
}