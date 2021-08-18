using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aijkl.VRChat.Posters.Shared;
using Aijkl.VRChat.Posters.Shared.Twitter;
using Aijkl.VRChat.Posters.Shared.Twitter.Models;
using Aijkl.VRChat.Posters.Twitter.Shared.Models;
using CoreTweet;
using Google.Apis.Drive.v3;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PosterController.Commands
{
    public class PromotionCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            CloudSettings cloudSettings = null;
            AppSettings appSettings = null;
            DriveService driveService = null;
            Tokens tokens = null;
            try
            {
                appSettings = AppSettings.Load("./Resources/appSettings.json");
                AnsiConsole.Status().Start(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.ServerCommunication)), x =>
                {
                    driveService = PosterHelper.CreateDriveService(File.ReadAllText(appSettings.AuthTokenPath));
                    cloudSettings = CloudSettings.Fetch(driveService, appSettings.FileID);
                    tokens = Tokens.Create(appSettings.TwitterParameters.APIKey, appSettings.TwitterParameters.APISecretKey, appSettings.TwitterParameters.AccessToken, appSettings.TwitterParameters.AccessTokenSecret);
                });
            }
            catch (Exception ex)
            {
                if (appSettings == null)
                {
                    AnsiConsole.MarkupLine(LanguageDataSet.CONFIG_FILE__LOAD_ERROR);
                }
                else
                {
                    AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.InitializationError)));
                }
                AnsiConsole.WriteException(ex);
                return 1;
            }

            AnsiConsole.Render(new FigletText(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.GeneralHeaderText))).LeftAligned().Color(Color.Red));
            AnsiConsole.Write("\n\n");

            for (int i = 0; i < 3; i++)
            {
                AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PromotionTweetId)));
                if (!long.TryParse(Console.ReadLine(), out long tweetId))
                {
                    AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.GeneralFormatError)));
                    continue;
                }
                AnsiConsole.Write("\n\n");

                AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PromotionDays)));
                if (!int.TryParse(Console.ReadLine(), out int days))
                {
                    AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.GeneralFormatError)));
                    continue;
                }
                AnsiConsole.Write("\n\n");

                AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PromotionDrawIndex)));
                if (!int.TryParse(Console.ReadLine(), out int drawIndex))
                {
                    AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.GeneralFormatError)));
                    continue;
                }
                AnsiConsole.Write("\n\n");

                IEnumerable<PosterParameters> posterParameters = AnsiConsole.Prompt(new MultiSelectionPrompt<PosterParameters>()
                    .Title(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PromotionPosterSelect)))
                    .NotRequired()
                    .AddChoices(cloudSettings.Posters)
                    .UseConverter(x => $"{Markup.Escape(x.Id.Substring(0, Math.Min(x.Id.Length, 20)))}"));

                if (!posterParameters.Any())
                {
                    AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.GeneralFormatError)));
                    continue;
                }

                try
                {
                    StatusResponse statusResponse = tokens.Statuses.Show(id: tweetId);

                    PromotionTweet promotionTweet = cloudSettings.Promotion.Tweets.FirstOrDefault(x => x.TweetId == statusResponse.Id);

                    if (promotionTweet != null)
                    {
                        promotionTweet.Deadline = DateTime.Now.AddDays(days);
                        promotionTweet.TargetPosterIds = posterParameters.Select(x => x.Id).ToArray();
                        promotionTweet.DrawIndex = drawIndex;
                        AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PromotionAlreadyExists)));
                    }
                    else
                    {
                        cloudSettings.Promotion.Tweets.Add(new PromotionTweet()
                        {
                            TweetId = tweetId,
                            Deadline = DateTime.Now.AddDays(days),
                            TargetPosterIds = posterParameters.Select(x => x.Id).ToArray(),
                            DrawIndex = drawIndex
                        });
                    }

                    Table table = new Table();
                    table.AddColumn("TweetText");
                    table.AddColumn("UserName");
                    table.AddColumn("TweetId");
                    table.AddRow(statusResponse.Text.Substring(0, Math.Min(statusResponse.Text.Length, 50)), statusResponse.User.Name, statusResponse.Id.ToString());
                    AnsiConsole.Render(table);
                }
                catch (Exception e)
                {
                    AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PromotionNotFound)));
                    AnsiConsole.WriteException(e);
                }

                try
                {
                    AnsiConsole.Status().Start(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.ServerCommunication)), x =>
                    {
                        cloudSettings.Push(driveService);
                    });
                    AnsiConsole.Write("\n\n");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.ServerCommunicationError)));
                    AnsiConsole.WriteException(ex);
                    return 1;
                }


                AnsiConsole.MarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.PromotionSuccess)));
                return 0;
            }

            return 1;
        }
    }
}
