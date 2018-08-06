using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HotelBot.Dialogs
{
    //Dialog to integrate with LUIS.ai
    //it has as parameters the appId, subscriptionId, domain and statging
    [LuisModel("aa3ba752-e062-4faa-a2eb-f8b41773d2e0", "7cd1dc3041554d83a62a79e2ecf0113f", domain: "westus.api.cognitive.microsoft.com", Staging =true)]
    [Serializable]
    public class LUISDialog : LuisDialog<BugReport>
    {
        private readonly BuildFormDelegate<BugReport> NewBugReport;

        //instantiate a new bug report
        public LUISDialog(BuildFormDelegate<BugReport> newBugReport)
        {
            //inject default values for the bug report
            this.NewBugReport = newBugReport;
        }

        //create the task to return i dont know what you mean for unknown messages
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry I don't know what you mean.");
            context.Wait(MessageReceived);
        }

        //create a task to map to greetingDialog
        [LuisIntent("GreetingIntent")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            context.Call(new GreetingDialog(), Callback);
        }

        //call callback method to be called after every dialog finishes running
        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        //create bug report intent
        [LuisIntent("NewBugReportIntent")]
        public async Task BugReport(IDialogContext context, LuisResult result)
        {
            var enrollmentForm = new FormDialog<BugReport>(new BugReport(), this.NewBugReport, FormOptions.PromptInStart);
            context.Call<BugReport>(enrollmentForm, Callback);
        }

        //create query bug type intent. 
        //This iterates throught he entity bug type ad compatte against of bug types
        [LuisIntent("QueryBugType")]
        public async Task QueryBugTypes(IDialogContext context, LuisResult result)
        {
            foreach (var entity in result.Entities.Where(Entity => Entity.Type == "BugType"))
            {
                var value = entity.Entity.ToLower();
                if(Enum.GetNames(typeof(BugType)).Where(a => a.ToLower().Equals(value)).Count() > 0)
                {
                    await context.PostAsync("Yes that is a bug type!");
                    context.Wait(MessageReceived);
                    return;
                }   
                else
                {
                    await context.PostAsync("I'm sorry that is not a bug type.");
                    context.Wait(MessageReceived);
                    return;
                }
            }
            await context.PostAsync("I'm sorry that is not a bug type.");
            context.Wait(MessageReceived);
            return;
        }

    }
}