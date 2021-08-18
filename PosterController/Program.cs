using PosterController.Commands;
using Spectre.Console.Cli;
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
                x.AddCommand<PromotionCommand>("promotion");
            });
            return commandApp.Run(args);
        }
    }
}
