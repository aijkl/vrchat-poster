using PosterController.Commands;
using Spectre.Console.Cli;
using System;
using System.Text;

namespace Aijkl.VRChat.Poster.Controller
{
    class Program
    {
        static int Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            CommandApp commandApp = new CommandApp();
            commandApp.Configure(x =>
            {
                x.AddCommand<MuteCommand>("mute");
            });
            return commandApp.Run(args);
        }
    }
}
