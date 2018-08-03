using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace HotelBot.Models
{

    //enum type to allow to fill the type of bug from a predefiennd attributestorag tlist
    public enum BugType
    {
        Security = 1,
        Crash = 2,
        Power = 3,
        Performance = 4,
        Usability = 5,
        SeriousBug = 6,
        Other = 7
    }

    public enum Reproducibility
    {
        Always = 1,
        Sometimes = 2,
        Rarely = 3,
        Unable = 4
    }

    //This class is to be used by the bot to fill a form, The fields will be populated by order they appear in here.
    [Serializable]
    public class BugReport
    {
        
        public string Title;

        [Prompt("Enter a description for your report")]
        public string Description;

        [Prompt("What is your first name?")]
        public string FirstName;

        [Describe("Surname")]
        public string LastName;

        [Prompt("What is the best date and time for a callback?")]
        public DateTime? BestTimeOfDayToCall;

        [Pattern("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$")]
        public string PhoneNumber;

        [Prompt("Please list the bug areas that best describe your issue. {||}")]
        public List<BugType> Bug;

        public Reproducibility Reproduce;


        //build a form
        public static IForm<BugReport> BuildForm()
        {
            return new FormBuilder<BugReport>().Message("Please fill out a bug").Build();
        }
    }
}