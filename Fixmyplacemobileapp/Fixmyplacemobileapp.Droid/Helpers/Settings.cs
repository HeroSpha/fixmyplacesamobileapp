
// Helpers/Settings.cs This file was automatically added when you installed the Settings Plugin. If you are not using a PCL then comment this file back in to use it.
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Fixmyplacemobileapp.Droid.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
        private static ISettings AppSettings => CrossSettings.Current;


        public static string RegistrationId
        {
            get => AppSettings.GetValueOrDefault(nameof(RegistrationId), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(RegistrationId), value);
        }

    }
}