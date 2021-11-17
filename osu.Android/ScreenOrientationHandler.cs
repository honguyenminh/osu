// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Android.Content.PM;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Configuration;
using osu.Game;

namespace osu.Android
{
    public class ScreenOrientationHandler : Component
    {
        private Bindable<bool> localUserPlaying;
        private Bindable<Orientation> orientation;

        [Resolved]
        private OsuGameActivity gameActivity { get; set; }

        [BackgroundDependencyLoader]
        private void load(OsuGame game, FrameworkConfigManager config)
        {
            localUserPlaying = game.LocalUserPlaying.GetBoundCopy();
            localUserPlaying.BindValueChanged(updateLock, true);
            orientation = config.GetBindable<Orientation>(FrameworkSetting.Orientation);
            orientation.BindValueChanged(value =>
            {
                gameActivity.RunOnUiThread(() =>
                {
                    gameActivity.RequestedOrientation = orientationToScreenOrientation(value.NewValue);
                });
            });
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
                    gameActivity.RequestedOrientation = orientationToScreenOrientation(orientation.Value);
            });
        }

        /// <summary>
        /// Convert <see cref="Orientation"/> framework setting enum to Android's <see cref="ScreenOrientation"/> enum
        /// </summary>
        /// <param name="orientation">Orientation</param>
        /// <returns><see cref="ScreenOrientation"/> enum to use with Android SDK</returns>
        private ScreenOrientation orientationToScreenOrientation(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Landscape:
                    return ScreenOrientation.UserLandscape;
                case Orientation.Portrait:
                    return ScreenOrientation.UserPortrait;
                case Orientation.Auto:
                default:
                    return ScreenOrientation.User;
            }
        }
    }
}
