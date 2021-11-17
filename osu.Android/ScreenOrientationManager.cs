// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Android.Content.PM;
using osu.Framework.Allocation;
using osu.Framework.Android.Platform;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Game;

namespace osu.Android
{
    public class ScreenOrientationManager : Component
    {
        private Bindable<bool> localUserPlaying;

        private AndroidOrientationManager manager;

        [Resolved]
        private OsuGameActivity gameActivity { get; set; }

        [BackgroundDependencyLoader]
        private void load(OsuGame game, FrameworkConfigManager config)
        {
            manager = new AndroidOrientationManager(config, gameActivity);

            localUserPlaying = game.LocalUserPlaying.GetBoundCopy();
            localUserPlaying.BindValueChanged(updateLock, true);
        }

        /// <summary>
        /// Control orientation lock while user is playing to avoid accidental screen rotate
        /// </summary>
        private void updateLock(ValueChangedEvent<bool> userPlaying)
        {
            gameActivity.RunOnUiThread(() =>
            {
                if (userPlaying.NewValue)
                    gameActivity.RequestedOrientation = ScreenOrientation.Locked;
                else
                    gameActivity.RequestedOrientation = manager.CurrentScreenOrientation;
            });
        }
    }
}
