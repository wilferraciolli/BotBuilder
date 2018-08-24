using System;
using System.Linq;
using HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using HotelBot.Models.Facebook;

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
                if (Enum.GetNames(typeof(BugType)).Where(a => a.ToLower().Equals(value)).Count() > 0)
                {
                    var replyMessage = context.MakeMessage();
                    replyMessage.Text = "Yes that is a bug type!";
                    var facebookMessage = new FacebookSendMessage();
                    facebookMessage.attachment = new FacebookAttachment();
                    facebookMessage.attachment.Type = FacebookAttachmentTypes.template;
                    facebookMessage.attachment.Payload = new FacebookPayload();
                    facebookMessage.attachment.Payload.TemplateType = FacebookTemplateTypes.generic;

                    var bugType = new FacebookElement();
                    bugType.Title = value;
                    switch (value)
                    {
                        case "security":
                            bugType.ImageUrl = "https://c1.staticflickr.com/9/8604/16042227002_1d00e0771d_b.jpg";
                            bugType.Subtitle = "This is a description of the security bug type";
                            break;
                        case "crash":
                            bugType.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/5/50/Windows_7_BSOD.png";
                            bugType.Subtitle = "This is a description of the crash bug type";
                            break;
                        case "power":
                            bugType.ImageUrl = "https://www.publicdomainpictures.net/en/view-image.php?image=1828&picture=power-button";
                            bugType.Subtitle = "This is a description of the power bug type";
                            break;
                        case "performance":
                            bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:High_Performance_Computing_Center_Stuttgart_HLRS_2015_07_Cray_XC40_Hazel_Hen_IO.jpg";
                            bugType.Subtitle = "This is a description of the performance bug type";
                            break;
                        case "usability":
                            bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:03-Pau-DevCamp-usability-testing.jpg";
                            bugType.Subtitle = "This is a description of the usability bug type";
                            break;
                        case "seriousbug":
                            bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:Computer_bug.svg";
                            bugType.Subtitle = "This is a description of the serious bug type";
                            break;
                        case "other":
                            bugType.ImageUrl = "https://commons.wikimedia.org/wiki/File:Symbol_Resin_Code_7_OTHER.svg";
                            bugType.Subtitle = "This is a description of the other bug type";
                            break;
                        default:
                            break;
                    }
                    facebookMessage.attachment.Payload.Elements = new FacebookElement[] { bugType };
                    replyMessage.ChannelData = facebookMessage;
                    await context.PostAsync(replyMessage);
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