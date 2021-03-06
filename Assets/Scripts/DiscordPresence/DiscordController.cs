using Blockstacker.Common;
using Discord;

namespace Blockstacker.DiscordPresence
{
    public class DiscordController : MonoSingleton<DiscordController>
    {
        private Discord.Discord discord;
        private static long ApplicationID => 953585016779202580;

        private void Update()
        {
            discord?.RunCallbacks();
        }

        private void OnDisable()
        {
            DisconnectFromDiscord();
        }

        private void OnApplicationQuit()
        {
            DisconnectFromDiscord();
        }

        public void ConnectToDiscord()
        {
            try
            {
                discord = new Discord.Discord(ApplicationID, (ulong) CreateFlags.NoRequireDiscord);
                var activityManager = discord.GetActivityManager();
                var activity = new Activity
                {
                    State = "Still Testing", Details = "Imagine you see me stacking blocks here"
                };
                activityManager.UpdateActivity(activity, _ => { });
            }
            catch (ResultException)
            {
                discord = null;
            }
        }

        public void DisconnectFromDiscord()
        {
            discord?.Dispose();
            discord = null;
        }
    }
}