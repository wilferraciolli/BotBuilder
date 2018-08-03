using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace HotelBot.Dialogs
{
    //This is the main dialog which will contain a chain of dialogs which will be called in sequence
    public class MainDialog
    {

        //switch between user inpout and call different dialogue by using a Chain
        public static readonly IDialog<string> dialog = Chain.PostToChain()
            .Select(msg => msg.Text)
            .Switch(
                new RegexCase<IDialog<string>>(new Regex("^hi", RegexOptions.IgnoreCase), (context, txt) =>
                {
                    //if the word ttyped in starts with hi then run the greeting dialog
                    return Chain.ContinueWith(new GreetingDialog(), AfterGreetingContinuation);
                }),
                new DefaultCase<string, IDialog<string>>((context, txt) =>
                 {
                     //Run the bug report dialogue
                     return Chain.ContinueWith(FormDialog.FromForm(BugReport.BuildForm, FormOptions.PromptInStart), AfterGreetingContinuation);
                 }))
            .Unwrap().
            PostToUser();

        //create ea method to run after the dialogue above is finished
        private async static Task<IDialog<string>> AfterGreetingContinuation(IBotContext context, IAwaitable<object> res)
        {
            var token = await res;
            var name = "User";
            context.UserData.TryGetValue<string>("Name", out name);

            return Chain.Return($"Thank you for using the bot {name}");
        }

    }
}