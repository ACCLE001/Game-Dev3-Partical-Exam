using UnityEngine;

namespace ThanethHamburut
{
    public static class GameData
    {
        public const string StudentCode = "672110095";
        public const string StudentName = "Thaneth Hamburut";
        public const string StudentNameEnglish = "Thaneth Hamburut";

        public static int CurrentScore = 0;
        public static string LastPlayedStage = "Stage1";

        public static float BGMVolume = 0.8f;
        public static float SFXVolume = 0.9f;

        public static void ResetGameData()
        {
            CurrentScore = 0;
        }
    }
}
