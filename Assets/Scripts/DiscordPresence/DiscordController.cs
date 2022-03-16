using Blockstacker.Common;
using UnityEngine;

namespace Blockstacker.DiscordPresence
{
    public class DiscordController : MonoSingleton
    {
        private long ApplicationID => 953585016779202580;
        private Discord.Discord discord;


        private void Update() => discord?.RunCallbacks();

        private void OnDisable() => Stop();

        public void Start()
        {
            discord = new Discord.Discord(ApplicationID, (ulong)Discord.CreateFlags.Default);
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                State = "Still Testing",
                Details = "Imagine you see me stacking blocks here"
            };
            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok) {
                    Debug.Log("Connected to Discord!");
                }
                else {
                    Debug.LogError("Discord couldn't be connected :(");
                }
            });
        }

        public void Stop()
        {
            discord?.Dispose();
            discord = null;
        }

    }
}