// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Android.Content.PM;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Configuration;
using osu.Framework.Android.Platform;
using osu.Game;

namespace osu.Android
{
    public class ScreenOrientationManager : AndroidOrientationManager
    {
        private Bindable<bool> localUserPlaying;

        [BackgroundDependencyLoader]
        private void load(OsuGame game, OsuGameActivity gameActivity, FrameworkConfigManager config)
        {
            LoadOrientation(config);
            GameActivity = gameActivity;
            localUserPlaying = game.LocalUserPlaying.GetBoundCopy();
            localUserPlaying.BindValueChanged(updateLock, true);

            OrientationBindable.BindValueChanged(value =>
            {
                GameActivity.RunOnUiThread(() =>
                {
                    CurrentScreenOrientation = OrientationToScreenOrientation(value.NewValue);
                    GameActivity.RequestedOrientation = CurrentScreenOrientation;
                });
            });
        }

        /// <summary>
        /// Control orientation lock while user is playing to avoid accidental screen rotate
        /// </summary>
        private void updateLock(ValueChangedEvent<bool> userPlaying)
        {
            GameActivity.RunOnUiThread(() =>
            {
                if (userPlaying.NewValue)
                    GameActivity.RequestedOrientation = ScreenOrientation.Locked;
                else
                    GameActivity.RequestedOrientation = CurrentScreenOrientation;
            });
        }
    }
}
