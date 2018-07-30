using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HotelBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog<object>
    {
        //override method to async receive messages
        public async Task StartAsync(IDialogContext context)
        {
            //return a response to the message
            await context.PostAsync("Hi, I am Wil's bot.");

            //tells the bot to wait and run the following method after sending the repsonse back to the user
            //this allow the bot to carry on the dialog
            context.Wait(MessageReceivedAsync);

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
            }

            if (string.IsNullOrEmpty(userName))
            {
                //query the user to pass their name
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("GetName", true);            
        }
            else
            {
                await context.PostAsync(String.Format("Hi {0}. How can I help ypou today?", userName));
            }

            //loop around until dialogue is finished
            //The bot always need a place to go, otherwise it throws an exception.
            context.Wait(MessageReceivedAsync);
        }
    }
}