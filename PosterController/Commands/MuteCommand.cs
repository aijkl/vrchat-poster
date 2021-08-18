using Aijkl.VRChat.Posters.Shared;
using Aijkl.VRChat.Posters.Shared.Twitter;
using Aijkl.VRChat.Posters.Shared.Twitter.Models;
using Google.Apis.Drive.v3;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CoreTweet;
using Status = CoreTweet.Status;
using Aijkl.VRChat.Posters.Shared.Expansion;

namespace PosterController.Commands
{
    public class MuteCommand : Command
    {
        public override int Execute(CommandContext context) 
        {            
            CloudSettings cloudSettings = null;
            LocalSettings localSettings = null;
            AppSettings appSettings = null;
            DriveService driveService = null;
            Tokens tokens = null;
            try
            {
                appSettings = AppSettings.Load("./Resources/appSettings.json");
                localSettings = LocalSettings.Load("./Resources/localSettings.json", false);
                AnsiConsole.Status().Start(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.ServerCommunication)), x =>
                {                    
                    driveService = PosterHelper.CreateDriveService(File.ReadAllText(appSettings.AuthTokenPath));
                    cloudSettings = CloudSettings.Fetch(driveService, appSettings.FileID);
                    tokens = Tokens.Create(localSettings.TwitterParameters.APIKey, localSettings.TwitterParameters.APISecretKey, localSettings.TwitterParameters.AccessToken, localSettings.TwitterParameters.AccessTokenSecret);
                });                
            }
            catch (Exception ex)
            {
                if(appSettings == null || localSettings == null)
                {
                    AnsiConsole.WriteLine(LanguageDataSet.CONFIG_FILE__LOAD_ERROR);
                }
                else
                {
                    AnsiConsole.WriteLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.InitializationError)));
                }
                AnsiConsole.WriteException(ex);
                return 1;
            }

            PosterParameters posterParameters = AnsiConsole.Prompt(new SelectionPrompt<PosterParameters>()
           .Title(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.MutePosterSelect)))
           .AddChoices(cloudSettings.Posters)
           .UseConverter(x => x.Title));
            
            Table table = new Table();
            table.BorderStyle = new Style(Color.Aqua);
            table.AddColumn(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PosterInfo)));
            table.AddColumn("");            
            table.AddRow(nameof(posterParameters.Title), $"[darkorange3_1]{posterParameters.Title}[/]");
            table.AddRow(nameof(posterParameters.Format), posterParameters.Format);
            table.AddRow(nameof(posterParameters.Page), posterParameters.Page.ToString());
            table.AddRow(nameof(posterParameters.Query), posterParameters.Query);
            table.AddRow(nameof(posterParameters.Quality), posterParameters.Quality.ToString());
            table.AddRow(nameof(posterParameters.Id), posterParameters.Id);
            table.AddRow(nameof(posterParameters.Lang), posterParameters.Lang);
            table.AddRow(nameof(posterParameters.RGBGroup.Background), string.Join(",", posterParameters.RGBGroup.Background));
            table.AddRow(nameof(posterParameters.RGBGroup.Tweet), string.Join(",", posterParameters.RGBGroup.Tweet));
            table.AddRow(nameof(posterParameters.TranslationLanguages), string.Join(",", posterParameters.TranslationLanguages));
            table.AddRow(nameof(posterParameters.Fonts), string.Join(",", posterParameters.Fonts.Select(x => $"{x.Key} {x.Value}")));
            AnsiConsole.Render(table);

            List<Status> statuses = null;
            AnsiConsole.Status().Start(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.ServerCommunication)), x =>
            {
               statuses = tokens.Search.Tweets(q: posterParameters.Query, count: 150, lang: posterParameters.Lang, locale: posterParameters.Locale, result_type: posterParameters.Sort, include_entities: true, tweet_mode: TweetMode.Extended).Where(x => !cloudSettings.Muted.IsMuted(x)).ToList();
            });
            List<Status> requireMuteStatuses = AnsiConsole.Prompt(new MultiSelectionPrompt<Status>()
            .Title(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.MuteTweetSelect)))
            .NotRequired()
            .AddChoices(statuses)
            .UseConverter(x => 
            {                
                string tweet = Markup.Escape(x.FullText.TrimText("shift-jis").Replace("\n", string.Empty));
                string userName = x.User.Name.TrimText("shift-jis");
                return $"{Markup.Escape(userName.Substring(0, Math.Min(userName.Length, 10)))} {tweet.Substring(0, Math.Min(tweet.Length, 45))}";
            }));
            
            foreach (var mutedStatus in requireMuteStatuses)
            {
                cloudSettings.Muted.Tweets.Add(new MutedTweet(mutedStatus.Id, null));
            }

            try
            {
                AnsiConsole.Status().Start(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.ServerCommunication)), x =>
                {
                    cloudSettings.Push(driveService);
                });
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.ServerCommunicationError)));
                AnsiConsole.WriteException(ex);
                return 1;
            }

            AnsiConsole.MarkupLine($"[green]{appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.MuteSuccess))}[/]");            
            Table muteStatusesTable = new Table();
            muteStatusesTable.AddColumn(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.MuteInfo)));
            muteStatusesTable.AddColumn("");
            foreach (var item in requireMuteStatuses)
            {
                muteStatusesTable.AddRow(nameof(item.FullText), Markup.Escape(item.FullText));
            }
            AnsiConsole.Render(muteStatusesTable);

            return 0;
        }        
    }
}
